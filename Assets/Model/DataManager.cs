using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Model
{
    public class DataManager
    {
        private static string dbPath = 
            Application.streamingAssetsPath + Path.DirectorySeparatorChar + "/db/db.json";
        private static string emptyDBPath = 
            Application.streamingAssetsPath + Path.DirectorySeparatorChar + "/db/db_empty.json";

        public static List<Person> GetPeople()
        {
            List<Person> people = GetDB().people;

            return people;
        }

        public static Person GetPersonById(string personid)
        {

            List<Person> people = GetDB().people;

            Debug.Log(people.ToString());

            foreach (Person person in people)
            {
                if (personid.Equals(person.id))
                {
                    return person;
                }
            }

            Debug.Log("person not found");
            return null;

        }

        public static List<Steps> GetAnswersOfCurrentScene(String scene)
        {
            List<Steps> questions;
            switch (scene)
            {
                case "MarketStand":
                    questions = GetDB().answers.marketstand;
                    break;
                case "Briefing":
                    questions = GetDB().answers.briefing;
                    break;
                default:
                    questions = new List<Steps>();
                    break;
            }

            return questions;
        }

        public static List<String> GetClasses()
        {

            List<Classes> classes = GetDB().classes;
            List<String> list = new List<String>();

            foreach (Classes clas in classes)
            {
                list.Add(clas.name);
            }

            return list;

        }

        public static Classes GetClasses(string className)
        {

            List<Classes> classes = GetDB().classes;
            foreach (Classes clas in classes)
            {
                if (clas.name.Equals(className))
                {
                    return clas;
                }
            }
            Debug.Log(classes.ToString());

            return null;

        }

        public static List<Item> GetItemsForClass(String className)
        {
            Classes clas = GetClasses(className);

            if (clas != null)
            {
                return clas.items;
            }

            return null;
        }

        public static List<Item> GetItemsByNameAndCategories(List<string> categories, string itemName)
        {
            List<Item> items = new List<Item>();
            List<Item> itemsToReturn = new List<Item>();
            foreach (string category in categories)
            {
                items = GetItemsForClass(category);

                string[] stringItems = itemName.Split('|');

                foreach (Item item in items)
                {
                    foreach (string name in stringItems)
                    {
                        if (item.name.Equals(name))
                        {
                            itemsToReturn.Add(item);
                        }
                    }
                }

            }
            return itemsToReturn;

        }

        public static List<Category> GetCategories()
        {

            List<Category> categories = GetDB().categories;

            Debug.Log(categories.ToString());

            //testing

            return categories;

        }

        public static Category GetCategoryByName(string categoryName)
        {

            List<Category> categories = GetDB().categories;

            foreach (Category category in categories)
            {
                if (category.name == categoryName)
                {
                    Debug.Log(categories.ToString());
                    return category;
                }
            }

            //testing
            Debug.Log("category not found");
            return null;

        }

        public static List<Prize> GetPrizes()
        {
            List<Prize> prizes = GetDB().prizes;
            return prizes;
        }

        public static Stars GetStars() {
            Stars stars = GetDB().Stars;
            return stars;
        }

        private static DatabaseData GetDB()
        {
            if (!File.Exists(dbPath))
            {
                // Create a file to write to.
                string createText = File.ReadAllText(emptyDBPath);
                File.WriteAllText(dbPath, createText);
            }

            string readText = File.ReadAllText(dbPath);
            DatabaseData dbData = JsonConvert.DeserializeObject<DatabaseData>(readText);
            return dbData;
        }

        private static void writeDB(string jsonData)
        {
            File.WriteAllText(dbPath, jsonData);
        }

        public static void newGame(string personId)
        {
            DatabaseData databaseData = GetDB();

            string playerLevel = PlayerPrefs.GetString("level");
            string totalHelp = PlayerPrefs.GetString("totalErrors");
            float totalTime = Time.time - PlayerPrefs.GetFloat("playTime");
            string minutes = Mathf.Floor(totalTime / 60).ToString("00");
            string seconds = (totalTime % 60).ToString("00");

            string.Format("{0}:{1}", minutes, seconds);

            int correctAnswers = PlayerPrefs.GetInt("score");
            Game newGame = new Game();
            newGame.id = databaseData.lastId.match;
            newGame.date = DateTime.Today.ToString("dd-MM-yyyy");
            newGame.level = playerLevel;
            newGame.totalHelp = totalHelp;
            newGame.totalTime = string.Format("{0}:{1}", minutes, seconds);
            newGame.correctAnswers = correctAnswers.ToString();
            databaseData.lastId.match = incrementId(databaseData.lastId.match);

            List<Person> people = databaseData.people;

            Person activePerson = new Person();
            foreach (Person person in people)
            {
                if (personId.Equals(person.id))
                {
                    activePerson = person;
                }
            }

            List<Game> gamesList = activePerson.games;
            gamesList.Add(newGame);
            activePerson.skipTutorial = "true";
            string jsonData = JsonConvert.SerializeObject(databaseData, Formatting.Indented);
            writeDB(jsonData);
        }

        private static string incrementId(string currentIdString)
        {
            int currentId = 0;

            Int32.TryParse(currentIdString, out currentId);

            currentId += 1;
            currentIdString = currentId.ToString();
            return currentIdString;
        }

        public static void SaveUpgradePlayerLevel(string playerId, string newLevel)
        {
            DatabaseData databaseData = GetDB();
            List<Person> people = databaseData.people;

            Person activePerson = new Person();
            foreach (Person person in people)
            {
                if (playerId.Equals(person.id))
                {
                    activePerson = person;
                }
            }

            activePerson.level = newLevel;
            string jsonData = JsonConvert.SerializeObject(databaseData, Formatting.Indented);
            writeDB(jsonData);
        }

        public static void RollbackDB() {
            string readText = File.ReadAllText(emptyDBPath);
            writeDB(readText);
        }

    }
}
