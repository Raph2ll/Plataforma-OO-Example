using Logger;
using Storage;
using Entities;
using Chat;
using Interfaces;
using System.Runtime.CompilerServices;


public class Program
{
    public static void Main(string[] args)
    {        
        ILogger logger = CreateConsoleLogger();
        //ILogger logger = DBFileLogger();
        //ILogger logger = CreateFileLogger();
        
        IStorage storage = CreateFileStorage(logger);
        //IStorage storage = CreateDatabaseStorage(logger);
        
        IChat chat = CreateChat(logger, storage);   

        Run(chat);
    }

    private static void Run(IChat chat)
    {
        Console.WriteLine($"> Send messages:");

        chat.SendMessage(new Message() { Text = "Hello", Author = "John" });
        chat.SendMessage(new Message() { Text = "Hi", Author = "Jane" });
        chat.SendMessage(new Message() { Text = "How are you?", Author = "John" });
        chat.SendMessage(new Message() { Text = "Fine", Author = "Jane" });
        chat.SendMessage(new Message() { Text = "Goodbye", Author = "John" });
        chat.SendMessage(new Message() { Text = "Bye", Author = "Jane" });

        var messages = chat.GetMessages();
        foreach (var message in messages)
        {
            Console.WriteLine($"> Message read: {message.ToJson()}");
        }
    }

    #region Factory methods
    private static IStorage CreateFileStorage(ILogger logger)
    {
        var storageConfig = new Dictionary<string,object>() { 
            {"path", "data/"},
            {"fileName", "data.json"} 
        };
        IStorage storage = new FileStorage();
        storage.Start(logger, storageConfig);
        return storage;
    }

    private static IStorage CreateDatabaseStorage(ILogger logger)
    {
        var storageConfig = new Dictionary<string,object>() { 
            {"fileName", "data/messages.db"}
        };
        IStorage storage = new DBStorage();
        storage.Start(logger, storageConfig);
        return storage;
    }

    private static ILogger CreateConsoleLogger()
    {
        var loggerConfig = new Dictionary<string,object>();
        ILogger logger = new ConsoleLogger();
        logger.Start(loggerConfig);
        return logger;
    }    

    private static ILogger CreateFileLogger()
    {        
        var loggerConfig = new Dictionary<string,object>() { 
            {"path", "data/logs.txt"}
        };
        ILogger logger = new FileLogger();
        logger.Start(loggerConfig);
        return logger;
    }

    private static ILogger DBFileLogger()
    {
        var loggerConfig = new Dictionary<string,object>() { 
            {"fileName", "data/logs.db"}
        };
        ILogger logger = new DBLogger();
        logger.Start(loggerConfig);
        return logger;
    }

    private static IChat CreateChat(ILogger logger, IStorage storage)
    {
        var chatConfig = new Dictionary<string,object>();
        IChat chat = new ChatService(storage);
        chat.Start(logger, chatConfig);
        return chat;
    }
    #endregion    
}


