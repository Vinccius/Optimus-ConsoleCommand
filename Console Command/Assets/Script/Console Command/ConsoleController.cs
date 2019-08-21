using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public delegate void CommandHandler(string[] args);

public class ConsoleController
{
    #region Event Declaration
    //used to communecate with ConsoleView

    public delegate void LogChangedHandler(string[] Log);
    public event LogChangedHandler LogChanged;

    public delegate void VisibilityChangedHandler(bool visible);
    public event VisibilityChangedHandler VisibilityChanged;
    #endregion
    
    // Object to hold information about each command

    class CommandRegistration
    {
        public string Command
        {
            get;
            private set;
        }
        public CommandHandler Handler
        {
            get;
            private set;
        }
        public string Help
        {
            get;
            private set;
        }

        public CommandRegistration(string command, CommandHandler handler, string help)
        {
            Command = command;
            Handler = handler;
            Help = help;
        }
    }

    private const int ScrollbackSize = 20;
    private Queue<string> Scrollback = new Queue<string>(ScrollbackSize);
    private List<string> CommandHistory = new List<string>();
    private Dictionary<string, CommandRegistration> Commands = new Dictionary<string, CommandRegistration>();

    public string[] Log
    {
        get;
        private set;
    }

    private const string RepeatCmdName = "!!";

    public ConsoleController()
    {        
        RegisterCommand("help", help, "Print this help.");
        //RegisterCommand("hide", hide, "Hide the console.");
        //RegisterCommand("RepeatCmdName", repeatCmdName, "Repeat last command.");
        //RegisterCommand("reload 1", reload, "Reload game.");
        RegisterCommand("reload", reload, "reload + Scene Index (int).");
        RegisterCommand("resetprefs", resetPrefs, "Reset & saves PlayerPrefs.");
    }

    void RegisterCommand(string Command, CommandHandler Handler, string Help)
    {
        Commands.Add(Command, new CommandRegistration(Command, Handler, Help));
    }

    public void AppendLogLine(string Line) // Exibe o Log no console
    {
        //Debug.Log(Line);

        if (Scrollback.Count >= ScrollbackSize)  //Se o número de elementos na fila for maior que 20...
        {
            Scrollback.Dequeue(); //Remove o último elemento da fila
        }

        Scrollback.Enqueue(Line); //Adiciona o último comando no final da fila
        ///
        Log = Scrollback.ToArray(); //Log(string[]) recebe a cópia dos elementos da fila
        ///
        if(LogChanged != null)  //Se o evento não for nulo
        {
            LogChanged(Log); //Evento recebe Log
        }
    }

    public void RunCommandString(string CommandString) //Recebe o comando do InputField
    {
        AppendLogLine(CommandString); //Aplica o append no parametro
        string[] CommandSplit = ParseArguments(CommandString); //
        string[] args = new string[0];

        
        if(CommandString.Length >= 2) 
        {
            int NumArgs = CommandSplit.Length - 1;
            args = new string[NumArgs];
            Array.Copy(CommandSplit, 1, args, 0, NumArgs);
        }
        //Debug.Log(CommandSplit[0].ToLower());
        RunCommand(CommandSplit[0].ToLower(), args); //ToLower() => Converte todos os caracteres em caracteres minusculos
        CommandHistory.Add(CommandString);
    }

    public void RunCommand(string command, string[] args)
    {
        CommandRegistration reg = null; 

        if(!Commands.TryGetValue(command, out reg)) // TryGetValue => Obtém o valor associado à chave especifica // TryGetValue(A chave do valor a ser obtido, out (Quando esse método retorna, ele contém o valor associado à chave especificada, caso a chave seja encontrada; caso contrário, o valor padrão para o tipo do parâmetro value). 
        {
            AppendLogLine(string.Format("Unknown command '{0}', type 'help' for list", command));
        }
        else
        {
            if(reg.Handler == null)
            {
                AppendLogLine(string.Format("Unable to process command '{0}', handler was null", command));
            }
            else
            {
                reg.Handler(args);
            }
        }
    }

    private static string[] ParseArguments(string commandString)
    {
        char[] c = commandString.ToCharArray();
        List<char> ParmChars = new List<char>(c); //Nova lista de "letras" (caracteres) => o número de elementos da lista é a quantidade de caracteres que contém a string 'commandString'.
        
        //bool InQuote = false;
        var Node = ParmChars.First();               

        //while(Node != null) //Enquanto Node não for nulo
        //{
        //    //var Next = Node;  //Recebe o próximo elemento da Lista
        //    //if (Node == '"') //Obtém o valor contido no elemento
        //    //{
        //    //    InQuote = !InQuote;
        //    //    ParmChars.Remove(Node); //Remove o elemento selecionado na lista
        //    //}

        //    //if (!InQuote && Node == ' ')
        //    //{
        //    //    Node = '\n';
        //    //}

        //    //Node = Next;
        //    //Debug.Log(ParmChars.First());

        //}

        char[] parmCharsArr = new char[ParmChars.Count]; //Cria uma array de caracteres que contém o número de elementos da LinkedList
        ParmChars.CopyTo(parmCharsArr, 0); //Copia os elementos da LinkedList a partir do elemento 0 da 'parmCharsArr'
        return (new string(parmCharsArr)).Split(new char[] { '\n' }); //retorna(nova string(parmCharsArr) => em seguida Split(Separa a cadeia de caracteres de entrada), remove as entradas vazias (expaços)
    }

    #region Command Handlers 
    void reload(string[] args)
    {        
        foreach(KeyValuePair<string, CommandRegistration> kvp in Commands)
        {
            string SceneIndex = kvp.Value.Command; //return - reload and 2
            string[] Index = Regex.Split(SceneIndex, @"\D+"); //Index[0] = reload, Index[1] = 2
            
            //for(int i = 0; i < Index.Length; i++)
            //{
            //    Debug.Log(Index[i]);
            //}

            foreach (string value in Index)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    int i = int.Parse(value);
                    Debug.Log(string.Concat("Index: ", i.ToString()));
                    SceneManager.LoadScene(i);
                }
                else
                {
                    AppendLogLine(string.Format("Unknown command '{0}', type 'help' for list", value));
                }
            }            
        }        
    }

    void resetPrefs(string[] args)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    
    void help(string[] args)
    {
        foreach(KeyValuePair<string, CommandRegistration> kvp in Commands)
        {
            string TheValues = kvp.Value.Help;
            string TheKeys = kvp.Key;

            string HelpLog = string.Concat(" ", TheKeys, " - ", TheValues);
            AppendLogLine(HelpLog);
        }
    }  
    
    #endregion
}
