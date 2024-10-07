using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

class Program
{
    // Variável estática (armazena em Static Storage)
    private static int raceCounter = 0;

    // Constante (armazena em Metadata Heap)
    private const string RaceType = "Formula 1";

    // Thread-Local Storage (cada thread terá sua própria cópia)
    private static ThreadLocal<int> threadLocalData = new(() => Thread.CurrentThread.ManagedThreadId);

    static void Main(string[] args)
    {
        // Exemplo de Stack
        int lapCount = 5; // Variável local (stack)
        Console.WriteLine($"Total laps: {lapCount}");

        // Exemplo de Heap (Classe instanciada)
        var driver = new Car("Lewis Hamilton", 1000);

        // Exemplo usando o segundo construtor (com modelo e potência)
        var model = new Car("Ferrari", 980, true);
        Console.WriteLine($"{driver.Driver} is driving a {model.Model} with {model.HorsePower} horsepower is available for a driver");

        // Incrementar a corrida (variável estática)
        raceCounter++;
        Console.WriteLine($"Race counter: {raceCounter}");

        // Exemplo de String (heap)
        string winner = "Lewis Hamilton"; // Strings são armazenadas no heap
        Console.WriteLine($"The winner is {winner}");

        // Exemplo de uso de Thread-Local Storage
        Thread t1 = new Thread(DisplayThreadLocalData);
        Thread t2 = new Thread(DisplayThreadLocalData);
        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        // Exemplo de arquivo mapeado em memória (Memory-Mapped File)
        SimulateMemoryMappedFile();

        // Forçar Garbage Collector (Heap Management)
        Console.WriteLine("Forçando coleta de lixo...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("Garbage Collection done.");

        Console.ReadKey();
    }

    // Exemplo de Thread-Local Storage
    static void DisplayThreadLocalData()
    {
        Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}, Thread-Local Value: {threadLocalData.Value}");
    }

    // Exemplo de Memory-Mapped File
    static void SimulateMemoryMappedFile()
    {
        using (var mmf = MemoryMappedFile.CreateNew("RaceResults", 1024))
        {
            using (var stream = mmf.CreateViewStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(12345); // Simulando escrita de resultados de corrida
                    Console.WriteLine("Race result written to memory-mapped file.");
                }
            }

            using (var stream = mmf.CreateViewStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    int raceResult = reader.ReadInt32();
                    Console.WriteLine($"Race result read from memory-mapped file: {raceResult}");
                }
            }
        }
    }
}

// Classe que será armazenada no Heap
public class Car
{
    public string Model { get; }
    public int HorsePower { get; }
    public string Driver { get; }

    // Construtor que recebe o driver e a potência do carro
    public Car(string driver, int horsePower)
    {
        Driver = driver;
        HorsePower = horsePower;
    }

    // Construtor que recebe o modelo do carro e a potência
    public Car(string model, int horsePower, bool isModel)
    {
        Model = model;
        HorsePower = horsePower;
        Driver = "Unknown"; // Driver será padrão
    }
}
