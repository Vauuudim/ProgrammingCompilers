# ProgrammingCompilers
ФИО: Витомский Вадим Евгеньевич

Вуз: ДВФУ

Направление подготовки: Прикладная информатика в компьютерном дизайне

Год: 2021

Анализируемый язык программирования: Pascal

Инструкции по запуску:

Анализатор представляет утилиту командной строки, принимающую в качестве параметров:
- Путь к файлу(ам) с исходным кодом;
- Ключ режима работы;
- Ключ для автоматического тестирования (необязательно).

Для работы автоматического тестирования требуется указывать путь к файлам для тестов, которые должны лежать в одной папке.

Файлы для сравнений должны включать в свои имена подстроку "check".

Выходные файлы в свои имена включают подстроку "_output" и создаются только при автоматическом тестировании.

Пример файлов для автоматического тестирования:

1.
- 00 - string.txt
- 00 - string_check.txt

2.
- qwertyuiop.txt
- qwertyuiop_check.txt

Примеры запуска:

1.
- dotnet run "C:\Users\User\Desktop\AutoTests1\00 - string.txt" -la
- Запустит лексический анализ текстового файла "00 - string.txt".

2.
- dotnet run C:\Users\User\Desktop\AutoTests1\ -la -at
- dotnet run "C:\Users\User\Desktop\AutoTests1" -la -at
- Запустит лексический анализ текстовых файлов в папке "AutoTests1" с автоматическим тестированием.

3.
- dotnet run "C:\Users\User\Desktop\AutoTests2\07 - all.txt" -sa
- Запустит синтаксический анализ текстового файла "07 - all.txt".

4.
- dotnet run C:\Users\User\Desktop\AutoTests2\ -sa -at
- dotnet run "C:\Users\User\Desktop\AutoTests2" -sa -at
- Запустит синтаксический анализ текстовых файлов в папке "AutoTests2" с автоматическим тестированием.