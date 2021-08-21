# 7 days to die mod WebBridge

### DEDICATED SERVERS ONLY

---

Этот мод отправляет игровые события на указанный url

---

Для сборки на ubuntu 20.04 доставить пакеты:
```bash
wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-3.1
```
Либо можно посмотреть тут https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-ubuntu

Пример конфигурационного файла (Будет создан автоматически при первом запуске, либо создать руками)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Settings>
  <!-- Эндпойнт хука вашего сервера -->
  <ApiUrl>http://localhost:5000/api/v1/hooks</ApiUrl>
  <!-- Токен, по которому можно распознать как сервер, так и поддельные запросы -->
  <WebToken>Don'tForgetReplaceThisToken</WebToken>
  <!-- Если опция установлена в true то мод шлёт так же события обновления -->
  <IsSendUpdateEvent>false</IsSendUpdateEvent>
  <!-- Если опция установлена в true то мод, для событий HookChat ожидает ответ 'ок' если -->
  <!-- можно показать сообщенеи другим пользователям и что либо другое если нельзя -->
  <IsMessageModerate>false</IsMessageModerate>
  <!-- Если опция установлена в true то мод, для событий PlayerLogin ожидает ответ 'ок' если -->
  <!-- можно пустить пользователя на сервер и что либо другое если нельзя -->
  <IsLoginControl>false</IsLoginControl>
</Settings>
```

Пример сервера на Flask (Python)
```python
from flask import Flask
from flask import request

app = Flask(__name__)


@app.route('/api/v1/hooks', methods=['POST'])
def hello_world():  # put application's code here
    print(request.form)

    return 'ok'

if __name__ == '__main__':
    app.run()

```

Пример лога событий (Актуально для мода версии 1.5)
```log
ImmutableMultiDict([('HookType', 'GameHook'), ('MessageType', 'GameAwake'), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('HookType', 'GameHook'), ('MessageType', 'StartDone'), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('HookType', 'KillHook'), ('victimId', '235'), ('victimType', 'Unknown'), ('AssailantId', ''), ('AssailantType', ''), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('HookType', 'PlayerHook'), ('MessageType', 'JoinedGame'), ('Message', ''), ('MainName', 'Player'), ('LocalizeMain', 'False'), ('SecondaryName', ''), ('LocalizeSecondary', 'False'), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('EntityID', '171'), ('PlayerName', 'Player'), ('CompatibilityVersion', 'Alpha 19.6'), ('SteamID', '14741474147414741'), ('SteamOwnerID', '14741474147414741'), ('IP', '192.168.1.3'), ('Ping', '0'), ('HookType', 'PlayerRespawnHook'), ('RespawnType', 'JoinMultiplayer'), ('Position', '-1170,61,475'), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('EntityID', '171'), ('PlayerName', 'Player'), ('CompatibilityVersion', 'Alpha 19.6'), ('SteamID', '14741474147414741'), ('SteamOwnerID', '14741474147414741'), ('IP', '192.168.1.3'), ('Ping', '0'), ('HookType', 'DisconnectHook'), ('Shutdown', 'False'), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('HookType', 'PlayerHook'), ('MessageType', 'LeftGame'), ('Message', ''), ('MainName', 'Player'), ('LocalizeMain', 'False'), ('SecondaryName', ''), ('LocalizeSecondary', 'False'), ('Token', "Don't forget replace this token")])
ImmutableMultiDict([('HookType', 'GameHook'), ('MessageType', 'Shutdown'), ('Token', "Don't forget replace this token")])
```