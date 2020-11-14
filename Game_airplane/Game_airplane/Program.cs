/*******************************************************************************
* Program.cs
*
* $Id: Program.cs v 1.0 2020/11/14 14:32 $
*
* Консольная игра, где сражаются 2 самолёта.
* Описывается 5 классов.
*   - Engine    - движок
*   - Player    - игрок
*   - IAirplane - интерфейсный класс
*   - Enemy     - противник
*   - Program   - main
*
* The following code is (c)copyright 2020, SWPSD Okolo IT.
* ALL RIGHT RESERVED
*******************************************************************************/
using System;

namespace Game_airplane
{
    /// <summary>
    /// Класс игрового движка.
    /// </summary>
    class Engine
    {
        private bool isPlaying;

        private Player player;
        private Enemy  enemy;

        /// <summary>
        /// Обработка ввода.
        /// </summary>
        private void Input()
        {
            string key = Console.ReadLine();
            switch (key)
            {
                case "1": // Выбрана атака
                    // Если атака не прошла успешно, значит игрок промахнулся.
                    if (player.BulletCheck()) {
                        player.Fire();
                        if (!enemy.hit(player.GetDamage()))
                            Console.WriteLine("Вы промахнулись...");
                        else
                            Console.WriteLine("Вы нанесли урон.");
                    }
                    else
                        Console.WriteLine("У вас закончились патроны.");
                    break;
                case "2": // Выбрано лечение.
                    player.Heal();
                    Console.WriteLine("Вы воспользовались лечением.");
                    break;
                case "3": // Выбрана покупка патронов.
                    player.BuyBullet();
                    Console.WriteLine("Вы купили патроны.");
                    break;
                case "4": // Выбран выход.
                    isPlaying = false;
                    break;
                default:
                    Console.WriteLine("Неверный ввод");
                    break;
            }
        }

        /// <summary>
        /// Обновление данных. 
        /// </summary>
        private void Update()
        {
            Random random = new Random();
            int action = random.Next(1, 10);
            int damage = enemy.GetDamage();

            // Шанс критического урона.
            if (random.Next(1, 10) == 2)
                damage = (int)(damage * 1.2);


            // Ход противника -------------------------------------------------
            // Противник выбрал лечение. (шанс 1 к 5)
            if (action % 5 == 0) { 
                enemy.Heal();
                Console.WriteLine("Противник воспользовался лечением.");
            }
            // Противник выбрал перезарядку. (шанс 1 к 10)
            else if (action == 1) { 
                enemy.BuyBullet();
                Console.WriteLine("Противник купил патроны.");
            }
            // Противник выбрал атаку.
            else {
                // Если атака не прошла успешно, значит противник промахнулся.
                if (enemy.BulletCheck()) {
                    enemy.Fire();
                    if (!player.hit(damage))
                        Console.WriteLine("Противник промахнулся.");
                    else
                        Console.WriteLine("Противник нанёс вам урон.");
                }
                else
                    Console.WriteLine("У противника закончились патроны.");
            }

            // Если у кого-то закончилась жизнь, 
            // закончить игру и вывести сообщение.
            if (!player.IsAlive()) {
                Console.WriteLine("Вы проиграли...");
                isPlaying = false;
            }
            if (!enemy.IsAlive()) {
                Console.WriteLine("Вы выиграли!");
                isPlaying = false;
            }
        }

        /// <summary>
        /// Вывод информации.
        /// </summary>
        private void Draw()
        {
            Console.Write('\n');

            // Вывести информацию об игроках.
            player.Draw();
            enemy.Draw();

            // Вывести вспомогательную информацию.
            Console.Write("1 - стрелять, 2 - лечиться, ");
            Console.Write("3 - перезарядка, 4 - выход\n");
            Console.Write("Выберите действие: ");
        }

        public Engine()
        {
            isPlaying = true;

            player = new Player();
            enemy  = new Enemy();

            // Вывод сообщения о начале игры.
            Console.Write("Добро пожаловать в игру!\n");
            Console.Write("Для выбора действий используйте цифры.\n");
            Console.Write("Удачи!\n\n");
        }

        /// <summary>
        /// Старт игрового движка.
        /// </summary>
        public void Run()
        {
            // Выполнять пока игра запущена.
            while (isPlaying)
            {
                Draw();
                Input();
                Update();
            }
        }
    }

    /// <summary>
    /// Класс игрока.
    /// </summary>
    class Player : IAirplane
    {
        private int health;
        private int armor;
        private int damage;
        private int bullet;

        private bool isAlive;
        private int maxHealth;
        public Player()
        {
            isAlive = true;

            maxHealth = 100;
            health = maxHealth;
            armor  = 100;
            damage = 10;
            bullet = 20;
        }

        /// <summary>
        /// Получен урон.
        /// </summary>
        /// <param name="value"> Урон.</param>
        /// <returns>Возвращает false при промахе.</returns>
        public bool hit(int value)
        {
            Random random  = new Random();

            if (BulletCheck()) {
                    // Шанс промаха. (20%)
                if (random.Next(1, 10) % 4 == 0)
                    return false;

                // Шанс критического урона (10%)
                int damage = value;
                if (random.Next(1, 10) == 2)
                    damage = (int)(value * 0.2);

                // Если есть броня, уменьшать её.
                if (armor > 0) {
                    armor -= damage;
                    // Броня не может иметь отрицательное значение.
                    if (armor <= 0) {
                        armor = 0;
                    }
                }
                else {
                    health -= damage;
                    // Жизнь не может иметь отрицательное значение.
                    if (health <= 0) {
                        health = 0;
                        isAlive = false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Жив игрок?
        /// </summary>
        /// <returns>Возвращает true если игрок жив.</returns>
        public bool IsAlive()
        {
            return isAlive;
        }

        /// <summary>
        /// Есть патроны?
        /// </summary>
        /// <returns>Возвращает true если патронов больше 0.</returns>
        public bool BulletCheck()
        {
            return bullet > 0;
        }

        /// <summary>
        /// Вывод информации.
        /// </summary>
        public void Draw()
        {
            Console.WriteLine($"Ваши показатели:  Жизнь: {health} Броня: {armor} Урон: {damage} Патроны: {bullet}");
        }

        /// <summary>
        /// Увеличить здоровье на 20%
        /// </summary>
        public void Heal()
        {
            health += (int)(maxHealth * 0.2);
            // Здоровье не может быть больше начального.
            if (health > maxHealth)
                health = maxHealth;
        }

        /// <summary>
        /// Увеличить количество патронов.
        /// </summary>
        public void BuyBullet()
        {
            bullet += 20;
        }

        /// <summary>
        /// Какой урон при атаке?
        /// </summary>
        /// <returns>Возвращает damage.</returns>
        public int GetDamage()
        {
            return damage;
        }

        /// <summary>
        /// Выстрелить.
        /// </summary>
        public void Fire()
        {
            // Патронов не может быть меньше 0.
            if(bullet > 0)
                --bullet;
        }
    }

    /// <summary>
    /// Класс противника.
    /// </summary>
    class Enemy : IAirplane
    {
        private int health;
        private int armor;
        private int damage;
        private int bullet;

        private int maxHealth;
        private bool isAlive;
        public Enemy()
        {
            isAlive = true;

            Random random = new Random();

            maxHealth = random.Next(100, 200);
            health = maxHealth;
            armor  = random.Next(50, 100);
            damage = random.Next(10, 20);
            bullet = random.Next(20, 40);
        }

        /// <summary>
        /// Получен урон.
        /// </summary>
        /// <param name="value"> Урон.</param>
        /// <returns>Возвращает false при промахе.</returns>
        public bool hit(int value)
        {
            Random random = new Random();

            // Шанс промаха. (20%)
            if (random.Next(1, 4) == 1)
                return false;

            // Шанс критического урона (10%)
            int damage = value;
            if (random.Next(1, 10) == 2)
                damage = (int)(value * 0.2);

           // Если есть броня, уменьшать её.
            if (armor > 0) {
                armor -= damage;
                // Броня не может иметь отрицательное значение.
                if (armor <= 0) {
                    armor = 0;
                }
            }
            else {
                health -= damage;
                // Жизнь не может иметь отрицательное значение.
                if (health <= 0) {
                    health = 0;
                    isAlive = false;
                }
            }
            return true;
        }

        /// <summary>
        /// Жив игрок?
        /// </summary>
        /// <returns>Возвращает true если игрок жив.</returns>
        public bool IsAlive()
        {
            return isAlive;
        }

        /// <summary>
        /// Есть патроны?
        /// </summary>
        /// <returns>Возвращает true если патронов больше 0.</returns>
        public bool BulletCheck()
        {
            return bullet > 0;
        }

        /// <summary>
        /// Вывод информации.
        /// </summary>
        public void Draw()
        {
            Console.WriteLine($"Показатели врага: Жизнь: {health} Броня: {armor} Урон: {damage} Патроны: {bullet}");
        }

        /// <summary>
        /// Увеличить здоровье на 20%
        /// </summary>
        public void Heal()
        {
            health += (int)(maxHealth * 0.2);
            // Здоровье не может быть больше начального.
            if (health > maxHealth)
                health = maxHealth;
        }

        /// <summary>
        /// Увеличить количество патронов.
        /// </summary>
        public void BuyBullet()
        {
            bullet += 20;
        }

        /// <summary>
        /// Какой урон при атаке?
        /// </summary>
        /// <returns>Возвращает damage.</returns>
        public int GetDamage()
        {
            return damage;
        }

        /// <summary>
        /// Выстрелить.
        /// </summary>
        public void Fire()
        {
            // Патронов не может быть меньше 0.
            if (bullet > 0)
                --bullet;
        }
    }

    /// <summary>
    /// Интерфейсный класс.
    /// </summary>
    interface IAirplane
    {
        public bool hit(int value);
        public bool BulletCheck();
        public bool IsAlive();
        public void Draw();
        public void Heal();
        public void BuyBullet();
        public int GetDamage();
        public void Fire();
    }

    /// <summary>
    /// Начальная точка для выполнения программы.
    /// Создаётся класс Engine и вызывается метод Run() для старта игры.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine();
            engine.Run();
        }
    }
}
