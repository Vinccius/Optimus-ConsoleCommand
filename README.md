# Console Command - C#

This system can be used to add commands (cheats) in your projet, for both players and developers. 

![Example Image](https://github.com/Vinccius/Optimus-ConsoleCommand/blob/master/ConsoleCommand%20print%202.png)

In this exemple in the image above, the string "help" represent the command itself. It should be entered in the console. The next parameter represents one void how this: 


    void resetPrefs(string[] args)
    {
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();
    }

> Warning: As you can see the void have a parameter "string[] args)". This, just like all void that represents a commmand should returns this same parameter to be called, otherwise it will generated an error in the code.


Lastly, the last parameter also is a string and represent the *help output*. The help is the commando that show in console all avalialble commands as well as your sintaxe. Example:

![Example Image](https://github.com/Vinccius/Optimus-ConsoleCommand/blob/master/ConsoleCommand%20print.png)
