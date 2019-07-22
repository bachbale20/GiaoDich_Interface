using System;

namespace GiaoDichInterface.demo
{
    public class MemberLogin : Login
    {
        public void doLogin()
        {
            Console.WriteLine("Nhập username");
            Console.WriteLine("Nhập password");
            Console.WriteLine("Login success!");
        }
    }
}