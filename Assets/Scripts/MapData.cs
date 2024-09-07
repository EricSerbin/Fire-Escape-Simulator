
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapData : MonoBehaviour
{
    
    [Range(1, 50)]
    public int width = 1;
    [Range(1, 50)]
    public int height = 1;
    //lets user adjust dimensions

    public TextAsset textAsset;
    public Texture2D textureMap;
    public List<string> GetTextFromFile(TextAsset tAsset)
    {
        List<string> lines = new List<string>();
        if (tAsset != null)
        {
            string textData = tAsset.text;
            string[] delimiters = { "\r\n", "\n" };
            lines = textData.Split(delimiters, System.StringSplitOptions.None).ToList();
        }
        else
        {
            Debug.Log("Mapdata cannot be initialized\n");
        }
        return lines;
    }
    //use manhattan distance for things like a*
    public List<string> GetTextFromFile()
    {
        return GetTextFromFile(textAsset);
    }

    public void SetDimensions(List<string> textLines)
    {
        height = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > width)
            {
                width = line.Length;
            }
        }
    }



    public List<string> GetMapFromTexture(Texture2D texture)
    {
        List<string> lines = new List<string>();

        if (texture != null)
        {
            for (int y = 0; y < texture.height; y++)
            {
                string newLine = "";
                for (int x = 0; x < texture.width; x++)
                {
                    if (texture.GetPixel(x, y) == Color.black)
                    {
                        newLine += "1";
                    }
                    else if (texture.GetPixel(x, y) == Color.white)
                    {
                        newLine += "0";
                    }
                    else
                    {
                        newLine += " ";
                    }
                }
                lines.Add(newLine);
                Debug.Log("the current line is: " + newLine + "\n");
            }
        }
        else
        {
            Debug.Log("Mapdata from Texture Error: invalid textasset\n");
        }
        return lines;
    }

    public int[,] MakeMap() //public int [,]
    {
        List<string> lines = new List<string>();

        if (textureMap != null)
        {
            lines = GetMapFromTexture(textureMap);
        }
        else//(textAsset!=null)
        {
            lines = GetTextFromFile(textAsset);

        }
        SetDimensions(lines);

        int[,] map = new int[width, height];
        Debug.Log("Height " + height + " width is " + width);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = (int)char.GetNumericValue(lines[y][x]);
                //Debug.Log(lines[y][x]+" ");
            }
            //Debug.Log("\n");
        }

        /* for (int y=0;y<height; y++)
         {
             for(int x=0;x<width;x++)
             {
                 map[x, y] = 0;//[x,y];
             }
         }*/
        //map[10, 5] = 3;
        map[10, 7] = 2;
        map[10, 8] = 3;
        map[2, 3] = 2;

        map[8, 4] = 2;

        map[13, 3] = 3;

        map[5, 4] = 3;
        map[5, 5] = 2;
        map[5, 6] = 1;
        map[4, 4] = 1;
        map[4, 5] = 1;
        map[4, 6] = 1;
        map[6, 4] = 1;
        map[6, 5] = 1;
        map[6, 6] = 1;

        map[11,2] = 1;
        map[16, 7] = 1;
        map[17, 7] = 1;
        map[18, 7] = 1;

        map[20, 4] = 2;
        map[20, 5] = 2;
        map[20, 6] = 2;
        map[21, 4] = 2;
        map[21, 5] = 2;
        map[21, 6] = 2;
        map[22, 4] = 3;
        map[22, 5] = 2;
        map[22, 6] = 2;

        return map;
    }
    void DisplayMap(int[,] map) //public int [,]
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Debug.Log(map[x, y]);
                //Console.WriteLine(map[x, y]);
            }
        }

    }
    
    public void Start()
    {
        int[,] mapInstance= MakeMap(); 
        DisplayMap(mapInstance);

    }

}
