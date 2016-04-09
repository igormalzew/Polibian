using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lab1
{
    /// <summary>
    /// Класс для организации словаря в виде дерева, и поиск слов в дереве
    /// </summary>
    static class MyDictionary
    {
        private static MyDictionaryTree myTree;
        public static void Init(string[] wordsArr)
        {
            // Инициализируем корень дерева
            myTree = new MyDictionaryTree() {nextLevel = new List<MyDictionaryTree>()};

            // Добавляем каждое слово из словаря в дерево
            foreach (var item in wordsArr)
            {
                Add(item.ToLower(), 0, myTree);
            }
        }

        /// <returns></returns>
        public static bool IsInit()
        {
            return myTree != null;
        }

        /// <summary>
        /// Рекурсивный метод заполнения дерева
        /// </summary>
        private static void Add(string item, int index, MyDictionaryTree three)
        {
            // когда уровень вложенности равен длине слова - сохраняем его
            if (item.Length == index)
            {
                three.word = item;
            }
            else
            {
                // Проверяем существование у узла потомка с заданным символом
                var isChild = three.nextLevel.Any(x => x.key == item[index]);
                // Добавляем при необходимости потомка
                if (!isChild) { three.nextLevel.Add(new MyDictionaryTree() { key = item[index], nextLevel = new List<MyDictionaryTree>()}); }

                // Только что созданного потомка(или существовавшего) передаем в рекурс. метод
                var child = three.nextLevel.FirstOrDefault(x => x.key == item[index]);
                if (child != null)
                {
                    Add(item, index + 1, child);
                }
            }
        }

        /// <summary>
        /// Поиск всех возможных вариантов слова по маске
        /// Вместо неизвестного символа передаем *
        /// </summary>
        /// <param name="mask">Вместо неизвестного символа передайте *</param>
        /// <param name="stopChar">массив символов, которые не могут находится на месте неизвестного символа в маске</param>
        /// <returns></returns>
        public static List<string> GetWordsByMask(string mask, char[] stopChar)
        {
            return GetWordsFromTree(mask, 0, myTree, stopChar).Where(x => x != null).ToList();
        }

        /// <summary>
        /// По маске выполняет поиск в дереве
        /// </summary>
        private static List<string> GetWordsFromTree(string mask, int index, MyDictionaryTree three, char[] stopChar)
        {
            // если глубина нахождения в дереве уже равна длине маски - возвращаем слово
            if (mask.Length == index)
            {
                return new List<string> {three.word};
            }
            else
            {
                if (mask[index] == '*')
                {
                    // Для неизвестного символа - берем все символы на данном уровне
                    // Вызываем для них рекурсивный метод - переходим не следующий уровень дерева
                    var result = new List<string>();
                    foreach (var child in three.nextLevel.Where(x => !stopChar.Contains(x.key)))
                    {
                        result.AddRange(GetWordsFromTree(mask, index + 1, child, stopChar));
                    }
                    return result;
                }
                else
                {

                    var child = three.nextLevel.FirstOrDefault(x => x.key == mask[index]);
                    if (child != null)
                    {
                        // переходим не следующий уровень дерева для определенного символа
                        return GetWordsFromTree(mask, index + 1, child, stopChar);
                    }
                    return new List<string>();
                }
            }
        }
    }

    /// <summary>
    /// Структура узла дерева
    /// </summary>
    public class MyDictionaryTree
    {
        public char key { get; set; }
        public string word { get; set; }
        public List<MyDictionaryTree> nextLevel { get; set; }
    }
}
