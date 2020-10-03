using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using EventCallback;

struct ArgumentResult
{
    public bool myBool;
    public string myString;
    public int myInt;
    public float myFloat;
}

public class Console : Control
{
    //The output text field for the consol
    TextEdit outputField;
    //The text input field for the console
    LineEdit inputField;
    //Refference to the command handler script in the console
    CommandHandler commandHandler;
    //The list of arguments paased to the command
    List<ArgumentResult> argumentResults = new List<ArgumentResult>();
    //The last command input
    List<string> commandHistory = new List<string>();
    int commandHistoryLine;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        outputField = GetNode<TextEdit>("Output");
        inputField = GetNode<LineEdit>("Input");
        commandHandler = GetNode<CommandHandler>("CommandHandler");
        //Set the focus on the input field
        inputField.GrabFocus();
    }

    public void OnInputTextEntered(String newText)
    {
        //Clear the input field when enter is pressed
        inputField.Clear();
        commandHistory.Add(newText);
        //Send the input to the command method 
        ProcessCommand(newText);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && @event.IsPressed())
        {
            if (keyEvent.Scancode == (int)KeyList.Up)
            {
                GoToCommandHistory(1);
            }
            if (keyEvent.Scancode == (int)KeyList.Down)
            {
                GoToCommandHistory(-1);
            }
        }
    }

    private void GoToCommandHistory(int offset)
    {
        commandHistoryLine += offset;
        commandHistoryLine = Mathf.Clamp(commandHistoryLine, 0, commandHistory.Count);
        if (commandHistoryLine < commandHistory.Count && commandHistory.Count > 0)
        {
            inputField.Text = commandHistory[commandHistoryLine];
        }
        inputField.CaretPosition = 9999;
    }

    private void OutputText(String text)
    {
        //Outputs all the old text from the output field wit hte new command added on the bottom of the list
        outputField.Text = outputField.Text + "\n" + text;
        //To set the scroll of the output field to the lowest value every time
        outputField.ScrollVertical = 99999;
    }

    private void ProcessCommand(String text)
    {
        //Spilt all the words that are sent for precessing and strip any empty strings
        string[] argumentsString = text.Split(" ", false);

        if (argumentsString.Length <= 0) return;

        String commandWord = argumentsString[0];
        //We remove the first element from the arguments array as it was the command
        argumentsString = argumentsString.Skip(1).ToArray();

        foreach (var command in commandHandler.ValidCommands)
        {
            //If we do not find the command word in the llist we notify the user that it does not exist
            if (command.Key == commandWord)
            {

                //If the nummber of arguments passed are not the same as needed then we give an error reply
                if (command.Value.Count == argumentsString.Length)
                {

                    //if this argument is set to false some of the arguments are incorrect and the method will not be called
                    bool argumentTypesCorrect = true;
                    //Run through all the arguments in the string and check if they corespond with what the method is looking for
                    for (int i = 0; i < argumentsString.Length - 1; i++)
                    {
                        //Here we check if all the arguments are valid types
                        if (!CheckType(argumentsString[i], command.Value[i])) argumentTypesCorrect = false;
                    }
                    //If all checks passed we call the method
                    if (argumentTypesCorrect)
                    {
                        //Converted arguments to a godot array to inject it into callv
                        Godot.Collections.Array argumentsArray = new Godot.Collections.Array();

                        foreach (String argument in argumentsString)
                        {
                            argumentsArray.Add(argument);
                        }
                        commandHandler.Callv(commandWord, argumentsArray);
                        
                        ConsoleInputEvent consoleInput = new ConsoleInputEvent();
                        //Populate the event message with info here
                        consoleInput.FireEvent();

                    }
                    else
                    {
                        OutputText("Arguments are of the wrong type");
                        foreach (var argument in command.Value)
                        {
                            OutputText(argument.ToString());
                        }
                    }
                }
                else
                {
                    OutputText("Number of command arguments not correct, looking for:");
                    foreach (var argument in command.Value)
                    {
                        OutputText(argument.ToString());
                    }
                }
                //Breaks out of the loop where it looks for valid commands when a vaid command is found, if we dont it keeps looking and gives errors
                break;
            }
            else if (command.Equals(commandHandler.ValidCommands.Last()))
            {
                OutputText("Command not recognised: " + commandWord);
            }
        }
        //Output the commands to the output text box
        OutputText(text);
    }

    private bool CheckType(String word, CommandHandler.Arguments type)
    {
        switch (type)
        {
            case CommandHandler.Arguments.ARG_FLOAT:
                if (word.IsValidFloat()) return true;
                else return false;
            case CommandHandler.Arguments.ARG_INT:
                if (word.IsValidInteger()) return true;
                else return false;
            case CommandHandler.Arguments.ARG_STRING:
                return true;
            case CommandHandler.Arguments.ARG_BOOL:
                word = word.ToUpper();
                if (word == "TRUE" || word == "FALSE") return true;
                else return false;
            default:
                return false;
        }
    }
}
