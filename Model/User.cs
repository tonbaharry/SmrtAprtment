using System;

namespace SmartApartmentData
{
    public class User{
        public string UserName { get;set;}
        public int Id {get;set;}
        public DateTime CreateDt {get;set;}
        public byte [] PasswordHash { get;set;}
        public byte [] PasswordSalt { get;set;}
    }
}