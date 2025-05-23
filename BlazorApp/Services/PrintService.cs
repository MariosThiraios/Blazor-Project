using BlazorApp.SeedWork;

namespace BlazorApp.Services
{
    public class PrintService
    {
        public void PrintName(Person person)
        {
            Console.WriteLine(person.Name);
        }
    }
}