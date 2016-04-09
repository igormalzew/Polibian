using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab1
{
    /// <summary>
    /// Класс для криптоанализа зашифрованного текста на основе статистических характеристик исходного текста
    /// </summary>
    internal class Decryption
    {
        private string Alphabet = " о";
        private List<string> WORDSWORDS = new List<string>(); //TODO Debug

        public string DecryptText(string sourceText, string DEBUG) //TODO Debug
        {
            // самые частые символы
            var mostFrequentLetters = GetMostFrequentLetters(sourceText);

            // Разбиваем на слова по самому частому символу
            var potencialyWords = sourceText.Split(mostFrequentLetters[0], '\r', '\n').Where(x => x != String.Empty).ToArray();

            var WORDS = DEBUG.Split(' ', '\r', '\n').Where(x => x != String.Empty).ToArray();  //TODO Debug

            // Отбираем слова содержащие все символы шифротекста
            var uniqueWords = GetUniqueWords(potencialyWords, mostFrequentLetters.ToList(), WORDS);

            // Потенциальный результат декодирования - таблица замен
            var dic = new Dictionary<char, char>();
            // Предполагаем, что второй по частотности символ - буква о
            //dic.Add(mostFrequentLetters[1], 'о');

            var raplaceTable = GetRaplaceTable(uniqueWords, dic);

            var likelyText = GetLikelyText(raplaceTable, sourceText, mostFrequentLetters[0]);

            return likelyText;
        }

        private string GetLikelyText(List<Dictionary<char, char>> raplaceTable, string sourceText, char space)
        {
            sourceText = sourceText.Replace('\r', space);
            sourceText = sourceText.Replace('\n', space);

            string result = String.Empty;
            int maxResult = 0;
            foreach (var item in raplaceTable)
            {
                item.Add(space, ' ');

                var charArr = sourceText.Select(x => item.ContainsKey(x) ? item[x] : '*').ToArray();
                var newText = String.Join(String.Empty, charArr);
                var newWords = newText.Split(' ');

                int count = 0;
                foreach (var newWord in newWords)
                {
                    if(newWord == String.Empty) continue;

                    count += MyDictionary.GetWordsByMask(newWord.Replace("*", ""), new char[0]).Count;
                }

                if (count > maxResult)
                {
                    maxResult = count;
                    result = newText;
                }
            }
            return result;
        }

        private char[] GetMostFrequentLetters(string text)
        {
            var dic = new Dictionary<char, int>();

            // считаем количество всех символов в тексте
            foreach (var item in text)
            {
                if(item == '\n' || item == '\r') continue;
                

                if (!dic.ContainsKey(item))
                {
                    dic.Add(item, 1);
                }
                else
                {
                    dic[item]++;
                }
            }

            var sortDic = dic.OrderByDescending(c => c.Value).ToList();

            return sortDic.Select(c => c.Key).ToArray();

        }

        private List<string> GetUniqueWords(string[] potencialyWords, List<char> mostFrequentLetters, string[] WORDS) //TODO Debug
        {
            WORDSWORDS.Clear(); //TODO Debug


            var randomRange = Polibian.GetRandomRange(potencialyWords.Length);
            var resultWords = new List<string>();

            for (int i = 0; i < randomRange.Length; i++)
            {
                if(mostFrequentLetters.Count == 0) break;

                bool flage = false;
                var item = potencialyWords[randomRange[i]];
                for (int j = 0; j < item.Length; j++)
                {
                    if (mostFrequentLetters.Contains(item[j]))
                    {
                        flage = true;
                        mostFrequentLetters.Remove(item[j]);
                    }
                }
                if (flage)
                {
                    resultWords.Add(item);
                    WORDSWORDS.Add(WORDS[randomRange[i]]);
                }

            }

            return resultWords;
        }

        private List<Dictionary<char, char>> GetRaplaceTable(List<string> uniqueWords, Dictionary<char, char> lettersByMask)
        {
            WORDSWORDS = WORDSWORDS.OrderByDescending(x => x.Length).ToList(); //TODO Debug

            var dicOutput = new List<Dictionary<char, char>>();
            GetPotencialyLetters(uniqueWords.OrderByDescending(x => x.Length).ToList(), 0, lettersByMask, ref dicOutput);

            return dicOutput;
        }

        private List<string> GetValidWords(List<string> wordsList, string maskWord)
        {
            var newWords = new List<string>();
            foreach (var item in wordsList)
            {
                bool flage = true;
                for (int i = 0; i < item.Length; i++)
                {
                    for (int j = i + 1; j < item.Length; j++)
                    {
                        if ((item[i] == item[j]) != (maskWord[i] == maskWord[j]))
                        {
                            flage = false;
                            i = item.Length;
                            j = item.Length;
                        }
                    }
                }
                if (flage) newWords.Add(item);
            }
            return newWords;
        }

        private void GetPotencialyLetters(List<string> uniqueWords, int index, Dictionary<char, char> lettersByMask, ref List<Dictionary<char, char>> dicOutput)
        {
            var wordByMask = GetMask(uniqueWords[index], lettersByMask);
            var words1 = MyDictionary.GetWordsByMask(wordByMask, lettersByMask.Values.ToArray());
            var newWords1 = GetValidWords(words1, uniqueWords[index]);

            // Для знаков препинания
            var words2 = MyDictionary.GetWordsByMask(wordByMask.Substring(0, wordByMask.Length-1), lettersByMask.Values.ToArray());
            var newWords2 = GetValidWords(words2, uniqueWords[index].Substring(0, uniqueWords[index].Length - 1));
            newWords2 = newWords2.Select(x => x += '*').ToList();

            var newWords = newWords1.Concat(newWords2).ToList();

            if (index == uniqueWords.Count - 1)
            {
                foreach (var item in newWords)
                {
                    dicOutput.Add(GetNewMask(lettersByMask, uniqueWords[index], item));
                }
            }
            else
            {
                foreach (var item in newWords)
                {
                    var mask = GetNewMask(lettersByMask, uniqueWords[index], item);
                    GetPotencialyLetters(uniqueWords, index + 1, mask, ref dicOutput);
                }
            }
        }

        private string GetMask(string word, Dictionary<char, char> lettersByMask)
        {
            var charArr = word.Select(x => lettersByMask.Keys.Contains(x) ? lettersByMask[x] : '*').ToArray();
            return String.Join(String.Empty, charArr);
        }

        private Dictionary<char, char> GetNewMask(Dictionary<char, char> lettersByMask, string word, string newWord)
        {
            var letters = lettersByMask.ToDictionary(entry => entry.Key, entry => entry.Value);

            for (int i = 0; i < word.Length; i++)
            {
                if (!letters.Keys.Contains(word[i]))
                {
                    letters.Add(word[i], newWord[i]);
                }
            }
            return letters;
        }
    }
}
