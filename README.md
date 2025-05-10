[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/xs3aclQL)

# m09-t2-pr1-threads-i-tasks-ArnauPascual7

## Part1

El Projecte té 3 carpetes, Files, que conté l'arxiu csv d'estadístiques, Models, que conté les classes de King i Chopsticks i Tools, que conté una Helper Class.

La classe King conté tots els atributs i mètodes que utilitzen els threads, i la classe Chopsticks només conté el id del palent, només està per a major comoditat.

La Helper Class conté mètodes que no cal que estiguin en el Program, com mètodes per a printar per pantalla els missatges dels reis.

Quan un rei agafa un palet aquest el bloqueja, fent que els altres haguin d'esperar a que aquest rei acabi d'utilitzar el palent.

Cap comensal arriba a 15s sense mejar, així que cap passa fam, el temps mitja en que els comensals estàn sense mejar és de 7 segons aproximadament.

Cada comensal menja una mitja de 13 vegades.

El record de vegades que ha mejat un comesal és de 15 vegades, mentre que el record de menys vegades és de 10.

## Part2



## Bibliografía

### Enlaços

> Raquel Alemán i Eduard Ruesga. (Sense data). T2. Processos. GoogleDocs. Recuperat el 6/5/2025 de https://docs.google.com/document/d/1MtD84t79YI1Fm-wjA_nPslMbRfye_c0NplgustQXdSo/edit?tab=t.0.

> Sense Autor. (Sense data). La instrucción lock: asegúrese del acceso exclusivo a un recurso compartido. Microsoft. Recuperat el 6/5/2025 de https://learn.microsoft.com/es-es/dotnet/csharp/language-reference/statements/lock.

> anuphunt. (10/7/2019). Space-Invaders. Github. Recuperat el 8/5/2025 de https://github.com/anuphunt/Space-Invaders.

> Sense Autor. (Sense data). Math.Clamp Método. Microsoft. Recuperat el 10/5/2015 de https://learn.microsoft.com/es-es/dotnet/api/system.math.clamp?view=net-9.0#system-math-clamp(system-int32-system-int32-system-int32).

### Consultes IA

> Com puc fer que el repintat de la consola es faci  a 20 Hz i el càlcul de les posicions dels elements del joc a 50Hz en aquest codi:
```
private const int WINDOW_WIDTH = 40;
private const int WINDOW_HEIGHT = 20;
private static bool checkWindowSize = true;
private static bool gameRunning = true;
private static Spaceship spaceship;

public static void Main(string[] args)
{
Console.Title = "Asterioids - Arnau Pascual";
Console.CursorVisible = false;

spaceship = new Spaceship(new Position(WINDOW_WIDTH / 2, WINDOW_HEIGHT - 2));
WriteSprite(spaceship.Sprite, spaceship.Position);

Task.Run(ConsoleSize);
Task.Run(SpaceshipMovement);

while (gameRunning) { }
}

public static async Task ConsoleSize()
{
while (checkWindowSize)
{
if (Console.WindowWidth != WINDOW_WIDTH || Console.WindowHeight != WINDOW_HEIGHT)
{
Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
}
await Task.Delay(100);
}
}

public static async Task SpaceshipMovement()
{
Position position = spaceship.Position;

while (gameRunning)
{
ConsoleKey key = Console.ReadKey(true).Key;

switch (key)
{
case ConsoleKey.A:
position.X--;
break;
case ConsoleKey.D:
position.X++;
break;
}

if (position.X < 1)
position.X = 1;
else if (position.X >= WINDOW_WIDTH)
position.X = WINDOW_WIDTH - 1;

WriteSprite(spaceship.Sprite, spaceship.Position, position);

spaceship.Position = position;
}
}

public static void WriteSprite(char sprite, Position oldPosition, Position newPosition)
{
CleanSprite(oldPosition);
WriteSprite(sprite, newPosition);
}

public static void WriteSprite(char sprite, Position position)
{
Console.SetCursorPosition(position.X, position.Y);
Console.Write(sprite);
}
public static void CleanSprite(Position position)
{
Console.SetCursorPosition(position.X, position.Y);
Console.Write(' ');
}
```
> Resposta
```
class Program
{
    private const int WINDOW_WIDTH = 40;
    private const int WINDOW_HEIGHT = 20;
    private static bool gameRunning = true;
    private static Spaceship spaceship;
    
    // Nous elements per a control de temps
    private static Stopwatch gameClock = new Stopwatch();
    private static TimeSpan lastRender;
    private static TimeSpan lastUpdate;
    private static readonly object consoleLock = new object();

    public static async Task Main(string[] args)
    {
        Console.Title = "Asterioids - Arnau Pascual";
        Console.CursorVisible = false;
        Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
        Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);

        spaceship = new Spaceship(new Position(WINDOW_WIDTH / 2, WINDOW_HEIGHT - 2));
        
        gameClock.Start();
        lastRender = gameClock.Elapsed;
        lastUpdate = gameClock.Elapsed;

        var inputTask = Task.Run(ProcessInput);
        var updateTask = Task.Run(GameLoop);
        var renderTask = Task.Run(RenderLoop);

        await Task.WhenAll(inputTask, updateTask, renderTask);
    }

    // Nova estructura de bucle del joc
    private static async Task GameLoop()
    {
        const double updateInterval = 1000 / 50.0; // 50 Hz
        while (gameRunning)
        {
            var currentTime = gameClock.Elapsed;
            var deltaTime = currentTime - lastUpdate;
            
            if (deltaTime.TotalMilliseconds >= updateInterval)
            {
                UpdateGame(deltaTime);
                lastUpdate = currentTime;
            }

            await Task.Delay(1);
        }
    }

    private static async Task RenderLoop()
    {
        const double renderInterval = 1000 / 20.0; // 20 Hz
        while (gameRunning)
        {
            var currentTime = gameClock.Elapsed;
            var deltaTime = currentTime - lastRender;
            
            if (deltaTime.TotalMilliseconds >= renderInterval)
            {
                lock (consoleLock)
                {
                    RenderGame();
                }
                lastRender = currentTime;
            }

            await Task.Delay(1);
        }
    }

    // Processat d'entrades millorat
    private static async Task ProcessInput()
    {
        while (gameRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                HandleInput(key);
            }
            await Task.Delay(10);
        }
    }

    private static void HandleInput(ConsoleKey key)
    {
        Position newPos = spaceship.Position;
        
        switch (key)
        {
            case ConsoleKey.A: newPos.X--; break;
            case ConsoleKey.D: newPos.X++; break;
            case ConsoleKey.Escape: gameRunning = false; break;
        }

        newPos.X = Math.Clamp(newPos.X, 1, WINDOW_WIDTH - 2);
        spaceship.Position = newPos;
    }

    private static void UpdateGame(TimeSpan deltaTime)
    {
        // Aquí aniria la lògica d'actualització dels asteroides
        // Exemple: Moviment d'asteroides, detecció de col·lisions
    }

    private static void RenderGame()
    {
        Console.SetCursorPosition(0, 0);
        // Redibuixar tota la pantalla cada frame
        Console.Clear();
        WriteSprite(spaceship.Sprite, spaceship.Position);
    }

    // Millora en l'escriptura amb lock
    public static void WriteSprite(char sprite, Position position)
    {
        if (position.X >= 0 && position.X < WINDOW_WIDTH && 
            position.Y >= 0 && position.Y < WINDOW_HEIGHT)
        {
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write(sprite);
        }
    }
}
```
