using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringFilter : Singleton<StringFilter>, IManagerBase
{
    public TextAsset FilterFile;
    //使用的数据：
    private static HashSet<string> hash = new HashSet<string>();
    private byte[] fastCheck = new byte[char.MaxValue];
    private BitArray charCheck = new BitArray(char.MaxValue);
    private int maxWordLength = 0;
    private int minWordLength = int.MaxValue;
    private static string[] badwords;



    //初始化数据的代码：将敏感词加入的hash表中
    public void Initial()
    {
        string filterString = FilterFile.text;
        filterString = filterString.Replace("\r\n", "\n");
        filterString = filterString.Replace("\n", "、");
        badwords = filterString.Split('、');

        foreach (string word in badwords)
        {
            maxWordLength = Math.Max(maxWordLength, word.Length);
            minWordLength = Math.Min(minWordLength, word.Length);

            for (int i = 0; i < 7 && i < word.Length; i++)
            {
                fastCheck[word[i]] |= (byte)(1 << i);
            }

            for (int i = 7; i < word.Length; i++)
            {
                fastCheck[word[i]] |= 0x80;
            }

            if (word.Length == 1)
            {
                charCheck[word[0]] = true;
            }
            else
            {
                hash.Add(word);
            }
        }
    }
    public void Release()
    {
        
    }
    //判断是否包含脏字的代码：
    public bool HasBadWord(string text)
    {
        int index = 0;

        while (index < text.Length)
        {
            if ((fastCheck[text[index]] & 1) == 0)
            {
                while (index < text.Length - 1 && (fastCheck[text[++index]] & 1) == 0) ;
            }

            if (minWordLength == 1 && charCheck[text[index]])
            {
                return true;
            }

            for (int j = 1; j <= Math.Min(maxWordLength, text.Length - index - 1); j++)
            {
                if ((fastCheck[text[index + j]] & (1 << Math.Min(j, 7))) == 0)
                {
                    break;
                }

                if (j + 1 >= minWordLength)
                {
                    string sub = text.Substring(index, j + 1);

                    if (hash.Contains(sub))
                    {
                        return true;
                    }
                }
            }

            index++;
        }

        return false;
    }
}