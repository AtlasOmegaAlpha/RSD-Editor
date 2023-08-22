namespace RSD_Editor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length >= 1)
            {
                Form1 f = new Form1();
                f.OpenFile(args[0]);
                Application.Run(f);
            }
            else
                Application.Run(new Form1());
        }
    }
}