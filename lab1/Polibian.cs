using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab1
{
    /// <summary>
    /// Предоставляет методы шифрования методом Полибианский квадрат для русского алфавита
    /// </summary>
    class Polibian
    {
        private static string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя.,!?;\" -'";
        private const int ENCRYPT_BASE = 6;
        private string KeyRange = String.Empty;

        public Polibian(string key)
        {
            KeyRange = key;
        }

        public static bool IsStringValid(string str)
        {
            var inputLow = str.ToLower();
            for (int i = 0; i < inputLow.Length; i++)
            {
                var letter = inputLow[i];
                if (letter == '\n') letter = '<';
                else if (letter == '\r') letter = '>';

                if (!Alphabet.Contains(letter))
                    return false;
            }
            return true;
        }

        public static string GetKey()
        {
            var randomRange = GetRandomRange(Alphabet.Length);
            var result = new StringBuilder();
            for (int i = 0; i < Alphabet.Length; i++)
            {
                result.Append(Alphabet[randomRange[i]]);
            }
            return result.ToString();

        }

        public static int[] GetRandomRange(int length)
        {
            var ran = new Random();

            var list = new List<int>(length);
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }   

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                var index = ran.Next(list.Count);
                result[i] = list[index];
                list.RemoveAt(index);
            }
            return result;
        }

        public string EncryptText(string text)
        {
            var result = new StringBuilder();
            text = text.ToLower();
            var lines = text.Split('\r','\n');

            foreach (var item in lines)
            {
                var str = new StringBuilder();
                if (item.Length != 0)
                {
                    str.Clear();
                    for (int i = 0; i < item.Length; i++)
                    {
                        str.Append(KeyRange[GetEncryptIndex(item[i])]);
                    }
                    result.Append(str.ToString()+"\r\n");
                }
            }

            return result.ToString();
        }

        public string DecryptText(string text)
        {
            var result = new StringBuilder();
            text = text.ToLower();
            var lines = text.Split('\n', '\r');

            foreach (var item in lines)
            {
                var str = new StringBuilder();
                if (item.Length != 0)
                {
                    str.Clear();
                    for (int i = 0; i < item.Length; i++)
                    {
                        str.Append(KeyRange[GetDecryptIndex(item[i])]);
                    }
                    result.Append(str.ToString() + "\r\n");
                }
            } 

            return result.ToString();
        }

        int GetEncryptIndex(char letter)
        {
            var index = KeyRange.IndexOf(letter);

            return (index + ENCRYPT_BASE) % Alphabet.Length;
        }

        int GetDecryptIndex(char letter)
        {
            var index = KeyRange.IndexOf(letter);

            return (index - ENCRYPT_BASE + Alphabet.Length) % Alphabet.Length;
        }
    }
}
