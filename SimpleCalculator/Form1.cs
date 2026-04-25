using System.Globalization;

namespace SimpleCalculator;

public partial class Form1 : Form
{
    private double _currentValue = 0;
    private double? _storedValue = null;
    private string? _pendingOperation = null;
    private bool _newEntry = true;
    private bool _hasDecimal = false;

    public Form1()
    {
        InitializeComponent();
        this.KeyPreview = true;
        this.KeyDown += Form1_KeyDown;
        this.KeyPress += Form1_KeyPress;
    }

    private void Button_Click(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        string text = btn.Text;

        switch (text)
        {
            case "0": case "1": case "2": case "3": case "4":
            case "5": case "6": case "7": case "8": case "9":
                InputDigit(text[0]);
                break;
            case ".":
                InputDecimal();
                break;
            case "±":
                Negate();
                break;
            case "%":
                Percent();
                break;
            case "1/x":
                Reciprocal();
                break;
            case "x²":
                Square();
                break;
            case "√x":
                SquareRoot();
                break;
            case "⌫":
                Backspace();
                break;
            case "CE":
                ClearEntry();
                break;
            case "C":
                ClearAll();
                break;
            case "+":
                PerformOperation("+");
                break;
            case "−":
                PerformOperation("-");
                break;
            case "×":
                PerformOperation("*");
                break;
            case "÷":
                PerformOperation("/");
                break;
            case "=":
                CalculateResult();
                break;
        }
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        // Skip digit keys when Shift is pressed (they produce symbols like *, (, etc. via KeyPress)
        if (e.Shift || e.Alt)
        {
            e.Handled = true;
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.D0: case Keys.NumPad0: InputDigit('0'); break;
            case Keys.D1: case Keys.NumPad1: InputDigit('1'); break;
            case Keys.D2: case Keys.NumPad2: InputDigit('2'); break;
            case Keys.D3: case Keys.NumPad3: InputDigit('3'); break;
            case Keys.D4: case Keys.NumPad4: InputDigit('4'); break;
            case Keys.D5: case Keys.NumPad5: InputDigit('5'); break;
            case Keys.D6: case Keys.NumPad6: InputDigit('6'); break;
            case Keys.D7: case Keys.NumPad7: InputDigit('7'); break;
            case Keys.D8: case Keys.NumPad8: InputDigit('8'); break;
            case Keys.D9: case Keys.NumPad9: InputDigit('9'); break;
            case Keys.Decimal: case Keys.OemPeriod: InputDecimal(); break;
            case Keys.Add: PerformOperation("+"); break;
            case Keys.Subtract: case Keys.OemMinus: PerformOperation("-"); break;
            case Keys.Multiply: PerformOperation("*"); break;
            case Keys.Divide: case Keys.Oem2: PerformOperation("/"); break;
            case Keys.Oemplus: CalculateResult(); break;
            case Keys.Enter: CalculateResult(); break;
            case Keys.Back: Backspace(); break;
            case Keys.Delete: ClearEntry(); break;
            case Keys.Escape: ClearAll(); break;
        }
        e.Handled = true;
    }

    private void Form1_KeyPress(object? sender, KeyPressEventArgs e)
    {
        switch (e.KeyChar)
        {
            case '*':
                PerformOperation("*");
                break;
            case '/':
                PerformOperation("/");
                break;
            case '+':
                PerformOperation("+");
                break;
            case '-':
                PerformOperation("-");
                break;
            case '=':
                CalculateResult();
                break;
            case '%':
                Percent();
                break;
            case '.':
            case ',':
                InputDecimal();
                break;
        }
        e.Handled = true;
    }

    private void InputDigit(char digit)
    {
        if (_newEntry)
        {
            txtDisplay.Text = digit == '0' ? "0" : digit.ToString();
            _newEntry = false;
            _hasDecimal = false;
        }
        else
        {
            if (txtDisplay.Text.Length < 15)
            {
                txtDisplay.Text += digit;
            }
        }
        UpdateCurrentFromDisplay();
    }

    private void InputDecimal()
    {
        if (_newEntry)
        {
            txtDisplay.Text = "0.";
            _newEntry = false;
            _hasDecimal = true;
        }
        else if (!_hasDecimal)
        {
            txtDisplay.Text += ".";
            _hasDecimal = true;
        }
        UpdateCurrentFromDisplay();
    }

    private void ClearEntry()
    {
        txtDisplay.Text = "0";
        _currentValue = 0;
        _newEntry = true;
        _hasDecimal = false;
    }

    private void ClearAll()
    {
        txtDisplay.Text = "0";
        _currentValue = 0;
        _storedValue = null;
        _pendingOperation = null;
        _newEntry = true;
        _hasDecimal = false;
        lblExpression.Text = "";
    }

    private void Backspace()
    {
        if (_newEntry) return;

        if (txtDisplay.Text.Length > 1)
        {
            char lastChar = txtDisplay.Text[^1];
            if (lastChar == '.') _hasDecimal = false;
            txtDisplay.Text = txtDisplay.Text[..^1];
        }
        else
        {
            txtDisplay.Text = "0";
            _newEntry = true;
            _hasDecimal = false;
        }
        UpdateCurrentFromDisplay();
    }

    private void Negate()
    {
        if (txtDisplay.Text == "0") return;

        if (txtDisplay.Text.StartsWith("-"))
            txtDisplay.Text = txtDisplay.Text[1..];
        else
            txtDisplay.Text = "-" + txtDisplay.Text;

        UpdateCurrentFromDisplay();
    }

    private void Percent()
    {
        _currentValue /= 100;
        txtDisplay.Text = FormatNumber(_currentValue);
        _newEntry = true;
    }

    private void Square()
    {
        _currentValue *= _currentValue;
        txtDisplay.Text = FormatNumber(_currentValue);
        _newEntry = true;
    }

    private void SquareRoot()
    {
        if (_currentValue < 0)
        {
            txtDisplay.Text = "Error";
            _newEntry = true;
            return;
        }
        _currentValue = Math.Sqrt(_currentValue);
        txtDisplay.Text = FormatNumber(_currentValue);
        _newEntry = true;
    }

    private void Reciprocal()
    {
        if (_currentValue == 0)
        {
            txtDisplay.Text = "Cannot divide by zero";
            _newEntry = true;
            return;
        }
        _currentValue = 1.0 / _currentValue;
        txtDisplay.Text = FormatNumber(_currentValue);
        _newEntry = true;
    }

    private void PerformOperation(string op)
    {
        if (_pendingOperation != null && !_newEntry)
        {
            CalculateResult();
        }
        else if (_pendingOperation == null)
        {
            _storedValue = _currentValue;
        }

        _pendingOperation = op;
        lblExpression.Text = FormatNumber(_storedValue!.Value) + " " + GetOperationSymbol(op) + " ";
        _newEntry = true;
    }

    private void CalculateResult()
    {
        if (_pendingOperation == null) return;

        double result;
        double a = _storedValue!.Value;
        double b = _currentValue;

        switch (_pendingOperation)
        {
            case "+":
                result = a + b;
                break;
            case "-":
                result = a - b;
                break;
            case "*":
                result = a * b;
                break;
            case "/":
                if (b == 0)
                {
                    txtDisplay.Text = "Cannot divide by zero";
                    lblExpression.Text = "";
                    _pendingOperation = null;
                    _storedValue = null;
                    _newEntry = true;
                    return;
                }
                result = a / b;
                break;
            default:
                result = b;
                break;
        }

        txtDisplay.Text = FormatNumber(result);
        _currentValue = result;
        _storedValue = result;
        lblExpression.Text = "";
        _pendingOperation = null;
        _newEntry = true;
    }

    private void UpdateCurrentFromDisplay()
    {
        if (double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
        {
            _currentValue = val;
        }
    }

    private string FormatNumber(double value)
    {
        if (double.IsInfinity(value) || double.IsNaN(value))
            return "Error";

        if (value == Math.Floor(value) && Math.Abs(value) < 1e15)
        {
            return value.ToString("0", CultureInfo.InvariantCulture);
        }

        string formatted = value.ToString("0.##########", CultureInfo.InvariantCulture);
        if (formatted.Length > 15)
        {
            formatted = value.ToString("G10", CultureInfo.InvariantCulture);
        }
        return formatted;
    }

    private string GetOperationSymbol(string op)
    {
        return op switch
        {
            "+" => "+",
            "-" => "−",
            "*" => "×",
            "/" => "÷",
            _ => op
        };
    }
}