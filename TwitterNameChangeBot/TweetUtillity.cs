using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using CoreTweet.Rest;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace TwitterNameBot {
    public class TweetUtillity {
        private string keyWord;
        private const string expression = "お前の名前は「(?<name>.*)」だ$";
        private readonly string ApiKey;
        private readonly string ApiSecret;

        public TweetUtillity(string apiKey, string apiSecret) {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
        }

        private Tokens GetAccessToken() {
            var accesTokenPath = "AccesToken.txt";
            var secretTokenPath = "AccesTokenSecret.txt";
            Tokens token;
            try {
                if (!File.Exists(accesTokenPath) || !File.Exists(secretTokenPath)) { // 初めてのログインの場合
                    var session = OAuth.Authorize(ApiKey, ApiSecret);
                    Process.Start(session.AuthorizeUri.AbsoluteUri); //ブラウザを開いてPINコードを取得
                    var pinCode = Console.ReadLine(); //ここでPINコードを入力
                    token = session.GetTokens(pinCode); //PINコードからトークンを取得

                    File.WriteAllText(accesTokenPath, token.AccessToken); //アクセスキーをファイルをファイルに書き出す
                    File.WriteAllText(secretTokenPath, token.AccessTokenSecret); //アクセスキーをファイルをファイルに書き出す

                    keyWord = token.ScreenName; //自分のID(ScreenName)を検索ワードとして登録
                    Console.WriteLine(keyWord);
                }
                else {
                    var accesToken = File.ReadAllText(accesTokenPath);  //アクセスキーをファイルから取得
                    var secretToken = File.ReadAllText(secretTokenPath); //アクセスキーをファイルから取得

                    token = Tokens.Create(ApiKey, ApiSecret, accesToken, secretToken);
                    var user = token.Account.VerifyCredentials(); //ユーザ情報を取得
                    keyWord = user.ScreenName;
                    Console.WriteLine($"{keyWord}");
                }

                return token;
            }
            catch (Exception e) {
                throw new Exception(e + "トークンの作成に失敗しました。");
            }
        }
        private Dictionary<Status, string> GetTargetTweet(Tokens tokens) {
            var result = tokens.Search.Tweets(count => 50, q => "to:" + keyWord); //自分のIDで50件検索

            var tweetTable = new Dictionary<Status, string>();

            foreach (var tweet in result) {
                var text = tweet.Text;
                if (!(tweet.CreatedAt.UtcDateTime - DateTime.UtcNow > new TimeSpan(0, -10, 0))) { //10分以内じゃないツイートは排除
                    continue;
                }

                if ((bool)tokens.Statuses.Show(id => tweet.Id).IsFavorited) { continue; } //ふぁぼ済みだったら排除

                Regex reg = new Regex(expression);
                Match match = reg.Match(text);
                if (match.Success == true) {
                    var matchText = match.Groups["name"].Value;
                    if (matchText.Length > 50 || matchText.Length == 0) { continue; }
                    tweetTable.Add(tweet, matchText);
                }
            }
            return tweetTable;
        }
        public void NameChangeAsync() {
            var tokens = GetAccessToken();
            while (true) {

                var tweetTable = GetTargetTweet(tokens); // 定型文のツイートを検索して取得

                var tweets = tweetTable
                                .GetKeys()
                                .ToList();

                if (!tweets.Any()) { //ツイートがなかったら
                  //Console.WriteLine("対象ツイートないよーーーん");
                    Thread.Sleep(60 * 1000);
                    continue;
                }
                var tweet = tweets.Random();  //ランダム

                try {
                    var name = tweetTable[tweet];
                    tokens.Favorites.Create(id => tweet.Id); //対象ツイートをふぁぼる
                    tokens.Account.UpdateProfile(name: name); //名前を変える
                    tokens.Statuses.Update(status => $"私の名前は「{name}」です。#NameChangeBot"); //名前変更の報告ツイート
                    Console.WriteLine("NewName:" + name);
                    Thread.Sleep(60 * 1000);
                }
                catch (Exception e) {
                    throw new Exception(e + "名前変更に失敗しました。");
                }

            }
        }
    }
}