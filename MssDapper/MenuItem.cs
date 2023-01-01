namespace MssDapper
{
    internal class MenuItem
    {
        public int Index { get; set; }
        public string? Title { get; set; }
        public Func<Task<bool>>? Example { get; set; }
        public bool IsInsertExample { get; set; } 

        public MenuItem( string? title, Func<Task<bool>>? example,int index=0, bool isInsertExample=false)
        {
            Index = index;
            Title = title;
            Example = example;
            IsInsertExample = isInsertExample;
        }

        public  void WriteToConsole()
        {
            if(IsInsertExample)
            {
               Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine($" {Index}. {Title}");
            Console.ResetColor();
        }
    }
}
