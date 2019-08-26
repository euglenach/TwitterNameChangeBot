# TwitterNameChangeBot
リプに反応してユーザーネームを変えるbot

# 環境
 C#,[CoreTweet](https://github.com/CoreTweet/CoreTweet)

# 簡単な使い方
   コマンドライン引数にAPI keyとAPI secret keyを指定して実行してください。
   初回の起動はログイン処理を挟みます。
   自動でブラウザが開くのでログインしたらアクセスナンバーをコマンドラインに入力します。
   
   このプログラムを動かしている間は1分毎に自分宛のリプを取得します。
   「お前の名前は「???」だ」というツイートに反応して、???部分の文字にユーザーネームを自動で変えてくれます。
   詳細は https://qiita.com/Euglenach/items/43b95b11bf03b94b8cff をご覧ください。
   
# 補足
  コマンドライン引数を使った実装のためDebugモードにしか対応してないしてないです。
