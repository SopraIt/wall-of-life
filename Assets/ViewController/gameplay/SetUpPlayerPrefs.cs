using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Model;

namespace SetUpPlayerPrefs
{
    public class SetUpPlayerPrefs : MonoBehaviour

    {

        public static void ResetPlayer()
        {
            PlayerPrefs.SetString("hand", "HandRight");
            PlayerPrefs.SetInt("CityStep", 0);
            PlayerPrefs.SetInt("BriefingStep", 0);
            PlayerPrefs.SetInt("MarketListStep", 0);
            PlayerPrefs.SetInt("MarketStandStep", 0);
            PlayerPrefs.SetInt("step", 0);
            PlayerPrefs.SetString("category1", "");
            PlayerPrefs.SetString("category2", "");
            PlayerPrefs.SetString("category3", "");
            PlayerPrefs.SetString("item1", "");
            PlayerPrefs.SetString("CorrectTextAnswer", "");
            PlayerPrefs.SetString("CorrectObj", "");
            PlayerPrefs.SetString("CorrectProduct", "");

            PlayerPrefs.SetInt("errorsCount", 0);
            PlayerPrefs.SetString("totalErrors", "0");

            PlayerPrefs.SetInt("score", 0);
            PlayerPrefs.SetString("win", "false");
            PlayerPrefs.SetString("prizeType", "");
            PlayerPrefs.SetString("prizeSource", "");
            PlayerPrefs.Save();
           //PlayerPrefs.DeleteAll();
        }

        public static void GetPlayerFromPerson(Person person)
        {
            PlayerPrefs.SetString("id", person.id);
            PlayerPrefs.SetString("hand", person.getHand);
            PlayerPrefs.SetString("level", person.level);
            PlayerPrefs.SetInt("matchesCount", person.games.Count);
            PlayerPrefs.SetString("skipTutorial", person.skipTutorial);
            PlayerPrefs.Save();
        }

        public static int GetPlayerLevel()
        {
            int playerLevel;
            System.Int32.TryParse(PlayerPrefs.GetString("level"), out playerLevel);
            return playerLevel;
        }
    }
}
