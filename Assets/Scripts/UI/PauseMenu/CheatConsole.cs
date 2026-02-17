using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Toggle();
        }
    }

    void Toggle()
    {
        isOpen = !isOpen;
        consoleRoot.SetActive(isOpen);

        if (isOpen)
        {
            inputField.ActivateInputField();
        }
    }

    public void OnSubmit()
    {
        string input = inputField.text;
        inputField.text = "";

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
}
