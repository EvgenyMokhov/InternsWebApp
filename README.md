Приложение на микросервисной архитектуре с использованием RabbitMQ + MassTransit в качестве транспорта между микросервисами.

Написан docker-compose для быстрого запуска всех сервисов, на бэкенде используются паттерны Repository и Saga на Api Gateway, который также выступает в роли оркестратора.

В приложении используются dll с классами, представляющими данные в том или ином виде (Dto, DbEntities, etc.) [source](https://github.com/EvgenyMokhov/ModelsInternsApp)

Если возникает ошибка при сборке проектов в docker-compose, проверьте наличие файла settings.json в папке ~/AppData/Roaming/Docker, если его там нет [скачайте его](https://drive.google.com/file/d/1XFNT5iuvn-GNmk0vLXrng0WuEbd-Aj2c/view?usp=drive_link)
