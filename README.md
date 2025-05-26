# メモアプリケーション

このアプリケーションは、Angular（フロントエンド）と.NET C#（バックエンド）で開発された練習用メモアプリケーションです。

## システム要件

- Docker
- Docker Compose
- .NET SDK 7.0（ローカル開発用）
- Node.js 18.x（ローカル開発用）

## 環境構築と起動方法

### Docker を使用する場合

1. リポジトリをクローン：

```bash
git clone <repository-url>
cd my-api
```

2. Docker Compose でアプリケーションを起動：

```bash
docker-compose up --build
```

3. ブラウザで以下の URL にアクセス：

- フロントエンド: http://localhost:4200
- バックエンド API: http://localhost:5000

### ローカル開発環境（Docker 不使用）

1. バックエンドの起動：

```bash
cd my-api
dotnet restore
dotnet run
```

2. フロントエンドの起動：

```bash
cd frontend
npm install
npm start
```

## データベースのバックアップと復元

### バックアップの作成

1. 以下のエンドポイントに GET リクエストを送信：

```bash
curl http://localhost:5000/api/database/backup -o backup.json
```

または、ブラウザで `http://localhost:5000/api/database/backup` にアクセスすることでもバックアップファイルをダウンロードできます。

### バックアップの復元

1. 以下のコマンドでバックアップデータを復元：

```bash
curl -X POST http://localhost:5000/api/database/restore \
  -H "Content-Type: application/json" \
  -d @backup.json
```

## 開発環境でのデータベース管理

- データベースファイルは `Data/prod-memos.db` に保存されます
- Docker 環境では、データベースは永続化ボリューム `db-data` に保存されます
- 新規デプロイ時にはデータがリセットされます

## 注意事項

- このアプリケーションは練習用であり、本番環境での使用は想定していません
- Azure Static Web Apps にデプロイされる場合、データベースは一時的なファイルシステムに保存され、デプロイ時にリセットされます
