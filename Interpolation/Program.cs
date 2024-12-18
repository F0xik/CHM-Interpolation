using System;

class Interpolation
{
    // Метод для вычисления интерполяционного полинома Лагранжа
    public static double LagrangePolynomial(double[] x, double[] y, double point)
    {
        if (x.Length != y.Length)
            throw new ArgumentException("Размер массивов x и y должен совпадать");

        int n = x.Length;
        double result = 0;

        for (int i = 0; i < n; i++)
        {
            double term = y[i];
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    term *= (point - x[j]) / (x[i] - x[j]);
                }
            }
            result += term;
        }

        return result;
    }

    // Метод для оценки погрешности интерполяции
    public static double EstimateError(Func<double, double> derivative, double[] x, double point, int n)
    {
        double M = 0;
        foreach (var xi in x)
        {
            M = Math.Max(M, derivative(xi));
        }

        // Вычисление произведения |x - x_i| для всех i
        double product = 1;
        foreach (var xi in x)
        {
            product *= Math.Abs(point - xi);
        }

        // Погрешность
        return M * product / Factorial(n + 1);
    }

    // Метод для вычисления факториала
    public static double Factorial(int n)
    {
        double result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    // Метод для вычисления кубического сплайна
    public static void CubicSpline()
    {
        // Заданные узлы и значения
        int n = 4; // Количество узлов
        double[] x = { 1, 2, 3, 4 }; // Значения x
        double[] y = { 1, 4, 9, 16 }; // Значения y
        double x0 = 2.5; // Точка, в которой требуется вычислить значение

        Console.WriteLine("Заданные узлы интерполяции и значения:");
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"x[{i}] = {x[i]}, y[{i}] = {y[i]}");
        }
        Console.WriteLine($"\nТочка для интерполяции: x0 = {x0}");

        // Промежуточные вычисления
        int N = n - 1;
        double[] h = new double[N];
        double[] a = new double[n];
        double[] b = new double[N];
        double[] c = new double[n];
        double[] d = new double[N];
        double[] alpha = new double[n];
        double[] l = new double[n];
        double[] mu = new double[n];
        double[] z = new double[n];

        // Коэффициенты a
        for (int i = 0; i < n; i++)
            a[i] = y[i];

        // Шаги h
        for (int i = 0; i < N; i++)
            h[i] = x[i + 1] - x[i];

        Console.WriteLine("\nШаги h:");
        for (int i = 0; i < N; i++)
            Console.WriteLine($"h[{i}] = {h[i]}");

        // Вычисление альфа
        for (int i = 1; i < N; i++)
            alpha[i] = (3 / h[i]) * (a[i + 1] - a[i]) - (3 / h[i - 1]) * (a[i] - a[i - 1]);

        // Прогонка
        l[0] = 1;
        mu[0] = z[0] = 0;

        for (int i = 1; i < N; i++)
        {
            l[i] = 2 * (x[i + 1] - x[i - 1]) - h[i - 1] * mu[i - 1];
            mu[i] = h[i] / l[i];
            z[i] = (alpha[i] - h[i - 1] * z[i - 1]) / l[i];
        }

        l[N] = 1;
        z[N] = c[N] = 0;

        for (int j = N - 1; j >= 0; j--)
        {
            c[j] = z[j] - mu[j] * c[j + 1];
            b[j] = (a[j + 1] - a[j]) / h[j] - h[j] * (c[j + 1] + 2 * c[j]) / 3;
            d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
        }

        Console.WriteLine("\nКоэффициенты сплайна:");
        for (int i = 0; i < N; i++)
        {
            Console.WriteLine($"Интервал [{x[i]}, {x[i + 1]}]:");
            Console.WriteLine($"a[{i}] = {a[i]}");
            Console.WriteLine($"b[{i}] = {b[i]}");
            Console.WriteLine($"c[{i}] = {c[i]}");
            Console.WriteLine($"d[{i}] = {d[i]}");
        }

        // Вычисление значения в точке x0
        double result = 0.0;
        for (int i = 0; i < N; i++)
        {
            if (x0 >= x[i] && x0 <= x[i + 1])
            {
                double dx = x0 - x[i];
                result = a[i] + b[i] * dx + c[i] * dx * dx + d[i] * dx * dx * dx;
                Console.WriteLine($"\nЗначение функции в точке x0 = {x0} составляет {result}");
                break;
            }
        }
    }

    // Главный метод
    public static void Main(string[] args)
    {
        Console.WriteLine("Выберите метод интерполяции:");
        Console.WriteLine("1 - Полином Лагранжа");
        Console.WriteLine("2 - Кубический сплайн");
        int choice = Convert.ToInt32(Console.ReadLine());

        switch (choice)
        {
            case 1:

                double[] x = { 2, 6, 10 };
                double[] y = { Math.Pow(Math.Log(2), 5.0 / 3), Math.Pow(Math.Log(6), 5.0 / 3), Math.Pow(Math.Log(10), 5.0 / 3) };
                double a = 6.5;


                Func<double, double> function = (x) => Math.Pow(Math.Log(x), 5.0 / 3);
                Func<double, double> derivative = (x) => (5.0 / 3) * Math.Pow(Math.Log(x), 2.0 / 3) / x;

                Console.WriteLine("Функция: f(x) = (ln(x))^(5/3)");

                double result = LagrangePolynomial(x, y, a);

                double error = EstimateError(derivative, x, a, x.Length - 1);

                Console.WriteLine($"Значение интерполяционного полинома Лагранжа в точке {a}: {result}");
                Console.WriteLine($"Оценка погрешности интерполяции в точке {a}: {error}");
                break;

            case 2:
                CubicSpline();
                break;

        }
    }
}
