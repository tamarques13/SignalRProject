using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CentralSystem
{
    public class User
    {
        public static string Name;

        public User(string name)
        {
            Name = name;
        } 

        public static string GetUser() => Name;
    }
}
