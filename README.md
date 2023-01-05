# EasyCAD - система автоматизации прочностных расчётов стержневых систем, испытывающих растяжение-сжатие.

## Конструкция
Конструкция представляет собой плоскую стержневую систему, составленную из прямолинейных стержней, последовательно соединённых друг с другом вдоль общей оси.
Каждый стержень i характеризуется длиной Li, площадью поперечного сечения Ai. 
Материал стержней должен характеризоваться модулем упругости Ei, допускаемым напряжением [σi]
## Нагрузки
На любое сечение конструкции могут быть наложены нулевые кинематические граничные условия (жёсткие опоры), запрещающие перемещения и повороты этих сечений во всех направлениях.
Конструкция может быть нагружена в глобальных узлах j статическими сосредоточенными продольными усилиями Fj.
Каждый стержень конструкции может быть нагружен постоянной вдоль его оси статической погонной нагрузкой qi.
## Задача
Система должня обеспечивать решение линейной задачи статики для плоских стержневых конструкций.
## Возможности программы
+ Система обеспечивает расчёт компонент напряжённо-деформированного состояния конструкции (продольные силы Nx, нормальные напряжения σx, перемещения ux);
+ Ввод массивов данных, описывающих конструкцию и внешние воздействия;
+ Формальнаю диагностика данных, описывающих конструкцию и внешние воздействия;
+ Визуализация конструкции и нагрузок;
+ Формирование файла проекта.
+ Формирование файла результатов расчёта;
+ Анализ результатов расчёта;
+ Отображение результатов расчёта в табличном виде;
+ Возможность получения всех компонент напряжённо-деформированного состояния в конкретном сечении конструкции;
+ отображение результатов расчёта в виде графиков, на оси ординат которых отложены интересующие пользователя компоненты напряжённо-деформированного состояния конструкции, а на оси абсцисс – локальные координаты стержней;
+ отображение результатов расчёта на конструкции в виде эпюр компонент напряжённо-деформированного состояния.
## Стек
+ C#, .NET6
+ WPF
+ EPPlus для работы с Excel
+ LiveCharts для работы с графиками

## Изображения
### Прошлая версия программы на WinForms
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/old.png)
### Обновленная версия на WPF
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/1.png)
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/2.png)
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/4.png)
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/5.png)
### Функции сохранения проекта, открытия и сохранения результатов с анализом
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/3.png)
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/results.png)
![](https://github.com/filippov-code/EasyCAD/blob/master/screenshots/saving.png)
