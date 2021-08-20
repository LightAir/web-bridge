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
  <ApiUrl>http://localhost:5000/api/v1/hooks</ApiUrl>
  <WebToken>Don'tForgetReplaceThisToken</WebToken>
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
