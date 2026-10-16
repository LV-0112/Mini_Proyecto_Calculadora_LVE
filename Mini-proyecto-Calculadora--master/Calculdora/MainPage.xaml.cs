using System.Globalization;

namespace Calculadora;

public partial class MainPage : ContentPage
{
	string expresion = "";

	bool esResultado = false;

	public MainPage()
	{
		InitializeComponent();
	}

	private void Numero_Clicked(object sender, EventArgs e)
	{
		Button boton = (Button)sender;
		string digito = boton.Text;

		if (esResultado || expresion == "Error")
		{
			expresion = "";
			esResultado = false;
		}

		if (expresion.EndsWith("%"))
		{
			return;
		}

		string actual = ObtenerNumeroActual();
		if (actual == "0")
		{
			expresion = expresion.Substring(0, expresion.Length - 1) + digito;
		}
		else
		{
			expresion = expresion + digito;
		}

		ActualizarPantalla();
	}

	private void Punto_Clicked(object sender, EventArgs e)
	{
		if (esResultado || expresion == "Error")
		{
			expresion = "";
			esResultado = false;
		}

		if (expresion.EndsWith("%"))
		{
			return;
		}

		string actual = ObtenerNumeroActual();

		if (actual.Contains("."))
		{
			return;
		}

		if (actual == "")
		{
			expresion = expresion + "0.";
		}
		else
		{
			expresion = expresion + ".";
		}

		ActualizarPantalla();
	}

	// Botones + - × ÷
	private void Operador_Clicked(object sender, EventArgs e)
	{
		Button boton = (Button)sender;
		string operador = boton.Text;

		if (expresion == "Error")
		{
			expresion = "";
		}

		esResultado = false;

		if (expresion == "")
		{
			// Si no hay nada, empiezo con 0 (como en el celular)
			expresion = "0" + operador;
		}
		else
		{
			char ultimo = expresion[expresion.Length - 1];

			if (EsOperador(ultimo))
			{
			
				expresion = expresion.Substring(0, expresion.Length - 1) + operador;
			}
			else
			{
				expresion = expresion + operador;
			}
		}

		ActualizarPantalla();
	}

	// Botón %
	private void Porcentaje_Clicked(object sender, EventArgs e)
	{
		if (expresion == "" || expresion == "Error")
		{
			return;
		}

		char ultimo = expresion[expresion.Length - 1];
		if (char.IsDigit(ultimo))
		{
			expresion = expresion + "%";
			esResultado = false;
			ActualizarPantalla();
		}
	}

	// Botón borrar (borra el último carácter)
	private void Borrar_Clicked(object sender, EventArgs e)
	{
		if (expresion == "Error")
		{
			expresion = "";
		}
		else if (expresion.Length > 0)
		{
			expresion = expresion.Substring(0, expresion.Length - 1);
		}

		esResultado = false;
		ActualizarPantalla();
	}

	// Botón C (limpia todo)
	private void Limpiar_Clicked(object sender, EventArgs e)
	{
		expresion = "";
		esResultado = false;
		ActualizarPantalla();
	}

	// Botón =
	private void Igual_Clicked(object sender, EventArgs e)
	{
		if (expresion == "" || expresion == "Error")
		{
			return;
		}

		char ultimo = expresion[expresion.Length - 1];
		if (EsOperador(ultimo))
		{
			expresion = expresion.Substring(0, expresion.Length - 1);
		}

		expresion = Calcular(expresion);
		esResultado = true;
		ActualizarPantalla();
	}


	// Dice si un carácter es un operador
	private bool EsOperador(char c)
	{
		return c == '+' || c == '-' || c == '×' || c == '÷';
	}

	// Regresa el número que se está escribiendo ahora (lo que está después del último operador)
	private string ObtenerNumeroActual()
	{
		int posicion = expresion.LastIndexOfAny(new char[] { '+', '-', '×', '÷' });
		return expresion.Substring(posicion + 1);
	}

	// Muestra la expresión en la pantalla
	private void ActualizarPantalla()
	{
		if (expresion == "")
		{
			lblPantalla.Text = "0";
		}
		else
		{
			lblPantalla.Text = expresion;
		}

		// Hago la letra más chica si el texto es largo
		if (lblPantalla.Text.Length <= 8)
		{
			lblPantalla.FontSize = 70;
		}
		else if (lblPantalla.Text.Length <= 12)
		{
			lblPantalla.FontSize = 45;
		}
		else
		{
			lblPantalla.FontSize = 30;
		}
	}

	private string Calcular(string texto)
	{
		List<double> numeros = new List<double>();
		List<char> operadores = new List<char>();
		string actual = "";
		double numero;

		// 1) Separo la expresión en números y operadores
		for (int i = 0; i < texto.Length; i++)
		{
			char c = texto[i];

			if (char.IsDigit(c) || c == '.')
			{
				actual = actual + c;
			}
			else if (c == '-' && i == 0)
			{
				actual = actual + c;
			}
			else if (c == '%')
			{

				if (!double.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out numero))
				{
					return "Error";
				}
				actual = (numero / 100).ToString("0.##########", CultureInfo.InvariantCulture);
			}
			else if (EsOperador(c))
			{
				if (!double.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out numero))
				{
					return "Error";
				}
				numeros.Add(numero);
				operadores.Add(c);
				actual = "";
			}
			else
			{
				return "Error";
			}
		}

		if (!double.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out numero))
		{
			return "Error";
		}
		numeros.Add(numero);

		int posicion = 0;
		while (posicion < operadores.Count)
		{
			if (operadores[posicion] == '×' || operadores[posicion] == '÷')
			{
				double resultadoParcial;

				if (operadores[posicion] == '×')
				{
					resultadoParcial = numeros[posicion] * numeros[posicion + 1];
				}
				else
				{
			
					if (numeros[posicion + 1] == 0)
					{
						return "Error";
					}
					resultadoParcial = numeros[posicion] / numeros[posicion + 1];
				}

				numeros[posicion] = resultadoParcial;
				numeros.RemoveAt(posicion + 1);
				operadores.RemoveAt(posicion);
			}
			else
			{
				posicion++;
			}
		}

		double total = numeros[0];
		for (int j = 0; j < operadores.Count; j++)
		{
			if (operadores[j] == '+')
			{
				total = total + numeros[j + 1];
			}
			else
			{
				total = total - numeros[j + 1];
			}
		}

		return total.ToString("0.##########", CultureInfo.InvariantCulture);
	}
}