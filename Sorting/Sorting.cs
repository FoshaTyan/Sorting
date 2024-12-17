using System.Diagnostics;

class Sorting
{
    static void Main()
    {
        int[] elementCounts = { 10, 1000, 10000, 100000 }; 
        var sortingAlgorithms = new Dictionary<string, Action<int[]>>
        {
            { "Selection Sort", SelectionSort },
            { "Merge Sort ", MergeSort },
            { "Count Sort ", CountSort },
            { "Radix Sort ", RadixSort },
            { "Basket Sort", BasketSort },
            { "Timsort    ", Timsort }
        };

        Console.WriteLine("\t\t" + string.Join("\t", elementCounts));

        foreach (var algo in sortingAlgorithms)
        {
            // Виведення назви алгоритму
            Console.Write(algo.Key);

            // Тестування для кожного розміру масиву
            foreach (int count in elementCounts)
            {
                // Генерація випадкового масиву
                int[] array = GenerateRandomArray(count, -100000, 100000);

                // Вимірювання часу сортування
                Stopwatch stopwatch = Stopwatch.StartNew();
                algo.Value(array); // Виклик відповідного алгоритму сортування
                stopwatch.Stop();

                Console.Write($"\t{stopwatch.ElapsedMilliseconds}ms");
            }

            Console.WriteLine();
        }

        //foreach (var algo in sortingAlgorithms)
        //{
        //    // Виведення назви алгоритму
        //    Console.Write(algo.Key);

        //    // Тестування для кожного розміру масиву
        //    foreach (int count in elementCounts)
        //    {
        //        // Генерація випадкового масиву
        //        int[] array = GenerateRandomArray(count, -10, 10);

        //        // Впорядковуємо масив за спаданням
        //        Array.Sort(array, (a, b) => b.CompareTo(a));

        //        // Вимірювання часу сортування
        //        Stopwatch stopwatch = Stopwatch.StartNew();
        //        algo.Value(array);  // Виклик відповідного алгоритму сортування
        //        stopwatch.Stop();

        //        Console.Write($"\t{stopwatch.ElapsedMilliseconds}ms");
        //    }

        //    Console.WriteLine();
        //}
    }



    static int[] GenerateRandomArray(int size, int minValue, int maxValue)
    {
        Random rand = new Random();
        return Enumerable.Range(0, size).Select(_ => rand.Next(minValue, maxValue)).ToArray();
    }

    static void SelectionSort(int[] array)
    {
        int n = array.Length;

        // Проходимо по кожному елементу масиву
        for (int i = 0; i < n - 1; i++)
        {
            // Знайдемо мінімальний елемент серед елементів, що йдуть після i
            int minIndex = i;
            for (int j = i + 1; j < n; j++)
            {
                if (array[j] < array[minIndex])
                {
                    minIndex = j;
                }
            }

            // Обмінюємо поточний елемент з мінімальним елементом
            if (minIndex != i)
            {
                (array[i], array[minIndex]) = (array[minIndex], array[i]);
            }
        }
    }
    static void MergeSort(int[] array)
    {
        if (array.Length <= 1) return;

        int mid = array.Length / 2;
        int[] left = array.Take(mid).ToArray();
        int[] right = array.Skip(mid).ToArray();

        MergeSort(left);
        MergeSort(right);
        Merge(array, left, right);
    }

    static void Merge(int[] array, int[] left, int[] right)
    {
        int i = 0, j = 0, k = 0;

        while (i < left.Length && j < right.Length)
        {
            if (left[i] <= right[j])
                array[k++] = left[i++];
            else
                array[k++] = right[j++];
        }

        while (i < left.Length) array[k++] = left[i++];
        while (j < right.Length) array[k++] = right[j++];
    }

    static void CountSort(int[] array)
    {
        int max = array.Max();
        int min = array.Min();
        int range = max - min + 1;

        int[] count = new int[range];
        int[] output = new int[array.Length];

        for (int i = 0; i < array.Length; i++)
            count[array[i] - min]++;

        for (int i = 1; i < range; i++)
            count[i] += count[i - 1];

        for (int i = array.Length - 1; i >= 0; i--)
        {
            output[count[array[i] - min] - 1] = array[i];
            count[array[i] - min]--;
        }

        Array.Copy(output, array, array.Length);
    }

    static void RadixSort(int[] array)
    {
        // Розділяємо масив на від'ємні та додатні числа
        var negativeNumbers = array.Where(x => x < 0).ToArray();
        var positiveNumbers = array.Where(x => x >= 0).ToArray();

        // Сортуємо додатні числа
        RadixSortHelper(positiveNumbers);

        // Сортуємо від'ємні числа (перетворюємо їх на додатні перед сортуванням)
        RadixSortHelper(negativeNumbers.Select(x => -x).ToArray());

        // Об'єднуємо масиви (від'ємні числа в зворотньому порядку)
        Array.Copy(negativeNumbers.OrderByDescending(x => x).ToArray(), 0, array, 0, negativeNumbers.Length);
        Array.Copy(positiveNumbers, 0, array, negativeNumbers.Length, positiveNumbers.Length);
    }

    static void RadixSortHelper(int[] array)
    {
        int max = array.Max();
        for (int exp = 1; max / exp > 0; exp *= 10)
            CountSortByDigit(array, exp);
    }

    static void CountSortByDigit(int[] array, int exp)
    {
        int n = array.Length;
        int[] output = new int[n];
        int[] count = new int[10];

        for (int i = 0; i < n; i++)
            count[(array[i] / exp) % 10]++;

        for (int i = 1; i < 10; i++)
            count[i] += count[i - 1];

        for (int i = n - 1; i >= 0; i--)
        {
            output[count[(array[i] / exp) % 10] - 1] = array[i];
            count[(array[i] / exp) % 10]--;
        }

        Array.Copy(output, array, n);
    }

    static void BasketSort(int[] array)
    {
        int max = array.Max();
        int min = array.Min();

        // Перевірка на однакові min і max значення (якщо всі елементи однакові)
        if (max == min)
        {
            Console.WriteLine("Усі елементи однакові, сортування не потрібне.");
            return;
        }

        int bucketCount = array.Length;
        List<int>[] buckets = new List<int>[bucketCount];

        // Ініціалізація кошиків
        for (int i = 0; i < bucketCount; i++)
            buckets[i] = new List<int>();

        // Розподіл елементів по кошиках
        foreach (int num in array)
        {
            // Коригуємо індекс так, щоб не було виходу за межі масиву
            int index = (int)((long)(num - min) * (bucketCount - 1) / (max - min));

            // Перевірка, чи індекс не виходить за межі
            if (index < 0 || index >= bucketCount)
            {
                Console.WriteLine($"Невірний індекс: {index} для числа {num}. Використано значення {bucketCount - 1}.");
                index = bucketCount - 1; // Фіксація індексу в межах масиву
            }

            buckets[index].Add(num);
        }

        // Обробка кошиків
        int k = 0;
        foreach (var bucket in buckets)
        {
            bucket.Sort();
            foreach (int num in bucket)
                array[k++] = num;
        }
    }

    static void Timsort(int[] array)
    {
        Array.Sort(array);
    }
}
