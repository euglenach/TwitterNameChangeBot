namespace TwitterNameBot {
    class Program {
        static void Main(string[] args) {
            var util = new TweetUtillity(args[0],args[1]);

            util.NameChangeAsync();
        }
    }
}