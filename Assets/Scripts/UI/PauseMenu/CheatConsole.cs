using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CheatConsole : MonoBehaviour
{

    [Header("References")]
    public GameObject consoleRoot;
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;

    private Dictionary<string, Action<string[]>> commands;
    private bool isOpen;

    void Awake()
    {
        consoleRoot.SetActive(false);

        commands = new Dictionary<string, Action<string[]>>();

        RegisterCommand("help", Help);
        RegisterCommand("clear", Clear);
        RegisterCommand("give_gold", GiveGold);
        RegisterCommand("teleport", Teleport);
        RegisterCommand("weapons", Weapons);
        RegisterCommand("item", Item);
        RegisterCommand("room", DevRoom);
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.BackQuote))
        //{
        //    Toggle();
        //}
    }

    public void Toggle(bool open)
    {
        isOpen = open;
        //consoleRoot.SetActive(isOpen);

        if (isOpen)
        {
            inputField.ActivateInputField();
        }else 
            inputField.DeactivateInputField();
    }

    public void OnSubmit(string input)
    {
        Debug.Log("OnSubmit:"+input);
        //input = inputField.text;
        inputField.text = "Enter command...";

        ProcessCommand(input);
        inputField.ActivateInputField();
    }

    void ProcessCommand(string input)
    {
        Print("> " + input);

        string[] split = input.Split(' ');
        string command = split[0].ToLower();

        if (commands.TryGetValue(command, out var action))
        {
            action(split);
        }
        else
        {
            Print("Unknown command.");
        }
    }

    void RegisterCommand(string name, Action<string[]> action)
    {
        commands.Add(name, action);
    }

    void Print(string message)
    {
        outputText.text += message + "\n";
    }

    // ===== Commands =====

    void Help(string[] args)
    {
        Print("Available commands:");
        foreach (var cmd in commands.Keys)
            Print("- " + cmd);
    }

    void Clear(string[] args)
    {
        outputText.text = "";
    }

    void GiveGold(string[] args)
    {
        if (args.Length < 2)
        {
            Print("Usage: give_gold <amount>");
            return;
        }

        if (int.TryParse(args[1], out int amount))
        {
            Print("Gave " + amount + " gold.");
            // Add to your game manager here
            ItemDropManager.instance.DropGold(GameObject.Find("Player").transform, amount);
        }
        else
        {
            Print("Invalid number.");
        }
    }

    void Teleport(string[] args)
    {
        if (args.Length < 4)
        {
            Print("Usage: teleport x y z");
            return;
        }

        if (float.TryParse(args[1], out float x) &&
            float.TryParse(args[2], out float y) &&
            float.TryParse(args[3], out float z))
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.transform.position = new Vector3(x, y, z);

            Print("Teleported player.");
        }
    }
    void Weapons(string[] args)
    {
        //if (args.Length < 2)
        //{
        //    Print("Usage: weapons all");
        //    return;
        //}
        TeleportData.playerManager.DebugAddWeapon();
    }
    void Item(string[] args)
    {
        if (args.Length < 2)
        {
            Print("Usage: item itemId");
            return;
        }
        ItemDetails itemDetails = ItemDropManager.GetDB().GetItem(args[1]);
        if (itemDetails != null)
            ItemDropManager.instance.DropItemById(args[1], TeleportData.playerManager.transform);
        else Print("itemIds:bread,cheese,ruby");
    }
    void DevRoom(string[] args)
    {
        if (args.Length < 2)
        {
            Print("Usage: room jerrydev");
            return;
        }

        switch (args[1].ToLower())
        {
            case "jerrydev":
                TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(1, 0, 0, 112);//JerryDev test dungeon
                break;
            case "alecdev":
                TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(5, -50, 21, -80);  // grassy island
                break;
            case "mesa":
                TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(3, 0, 10, 0); // Mesa Town
                break;
            case "tower":
                TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(2, 0, 140, 0); // Tower in ocean
                break;
            case "dungeon":
                TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(15);  // tower level select
                break;
            default:
                Print("Invalid=" + args[1] + " Valid=jerrydev/AlecDev/mesa/tower/dungeon");
                Debug.Log("Invalid=" + args[1]+ " Valid=jerrydev/AlecDev/mesa/tower/dungeon"); break;
        }
    }
}
