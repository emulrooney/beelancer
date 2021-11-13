using Godot;
using System;
using System.Collections.Generic;

public class Hive
{
    public string HiveName { get; private set; }
    public KeyValuePair<ResourceTypeEnum, float> Buying { get; private set; }
    public KeyValuePair<ResourceTypeEnum, float> Selling { get; private set; }
    
    private ResourceTypeEnum[] _validResources = new ResourceTypeEnum[]
    {
        ResourceTypeEnum.RedPollen,
        ResourceTypeEnum.GreenPollen,
        ResourceTypeEnum.BluePollen
    };

    private string[] _hiveNamePrefixes = new string[]
    {
        "Petunia",
        "Honey",
        "Rose",
        "Daisy",
        "Nectar",
        "Hornet",
        "Bumble",
        "Stinger",
        "Queen's ",
        "Drone's ",
        "Gold",
        "Stem",
        "Petal",
        "Windy ",
        "Meadow",
        "Leaf",
        "Bark",
        "Tree",
        "Butter",
        "Snapdragon",
        "Thistle",
        "Wasp",
        "Bee",
        "Bugg",
        "Bug",
        "Bird",
        "Cedar",
        "Birch"
    };

    private string[] _hiveNameBases = new string[]
    {
        "Hollow",
        "Home",
        "Town",
        "Wood",
        "Creek",
        "Yard",
        "Field",
        "Castle",
        "Fort",
        "House",
        "Ville",
        "Villa",
        "Road",
        "Post",
        "Market"
    };

    public Hive()
    {
        var notSold = Game.Random.Next(0, 2);
        ResourceTypeEnum? buy = null;
        ResourceTypeEnum? sell = null;
        
        for (int i = 0; i < _validResources.Length; i++)
        {
            if (i != notSold)
            {
                if (buy == null)
                {
                    buy = (ResourceTypeEnum)i;
                }
                else
                {
                    sell = (ResourceTypeEnum)i;
                }
            }
        }

        if (Game.Random.Next(0, 1) == 1)
        {
            (buy, sell) = (sell, buy);
        }

        float buyRate = (float)(100 + Game.Random.Next(-10, 10)) / 100;
        float sellRate = (float)(100 + Game.Random.Next(-10, 10)) / 100;
        
        GD.Print("Buy: " + buyRate);

        Buying = new KeyValuePair<ResourceTypeEnum, float>(buy.Value, buyRate);
        Selling = new KeyValuePair<ResourceTypeEnum, float>(sell.Value, sellRate);
        HiveName = _hiveNamePrefixes[Game.Random.Next(0, _hiveNamePrefixes.Length - 1)]
                   + _hiveNameBases[Game.Random.Next(0, _hiveNameBases.Length - 1)]
                   + " Hive";
    }

}
