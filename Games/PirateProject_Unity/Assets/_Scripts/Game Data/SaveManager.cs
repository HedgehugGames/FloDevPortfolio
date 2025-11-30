using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using StarterAssets;

public static class SaveManager
{
   public static string GetPath(int slot)
   {
      return Application.persistentDataPath + $"/GameData_Slot{slot}.bin";
   }

   public static void SaveData(GameObject player, int slot)
   {
      BinaryFormatter formatter = new BinaryFormatter();
      string path = GetPath(slot);
      FileStream stream = new FileStream(path, FileMode.Create);

      GameData data = new GameData(player, InventoryManager.Instance.items);
      formatter.Serialize(stream, data);
      stream.Close();
      
      Debug.Log($"Game saved to slot {slot} at path: {path}");
      Debug.Log($"Saved Position: {data.position[0]}, {data.position[1]}, {data.position[2]}");
      Debug.Log($"Saved Coins: {data.totalCoins}");

      for (int i = 0; i < data.itemNames.Count; i++)
      {
         Debug.Log($"Saved Item: {data.itemNames[i]}, Count: {data.itemCounts[i]}");
      }
   }
   
   public static GameData Load( int slot)
   {
      string path = GetPath(slot);
      if (File.Exists(path))
      {
         BinaryFormatter formatter = new BinaryFormatter();
         FileStream stream = new FileStream(path, FileMode.Open);
         GameData data = formatter.Deserialize(stream) as GameData;
         stream.Close();
         
         Debug.Log($"Game loaded from slot {slot} at path: {path}");
         Debug.Log($"Loaded Position: {data.position[0]}, {data.position[1]}, {data.position[2]}");
         Debug.Log($"Loaded Coins: {data.totalCoins}");

         for (int i = 0; i < data.itemNames.Count; i++)
         {
            Debug.Log($"Loaded Item: {data.itemNames[i]}, Count: {data.itemCounts[i]}");
         }
         
         return data;
      }
      else
      {
         Debug.Log("Save file not found in " + path);
         return null;
      }
   }
}
