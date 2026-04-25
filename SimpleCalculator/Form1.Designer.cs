namespace SimpleCalculator;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TextBox txtDisplay;
    private System.Windows.Forms.Label lblExpression;
    private System.Windows.Forms.Button btnPercent;
    private System.Windows.Forms.Button btnCE;
    private System.Windows.Forms.Button btnC;
    private System.Windows.Forms.Button btnBack;
    private System.Windows.Forms.Button btnReciprocal;
    private System.Windows.Forms.Button btnSquare;
    private System.Windows.Forms.Button btnSqrt;
    private System.Windows.Forms.Button btnDivide;
    private System.Windows.Forms.Button btn7;
    private System.Windows.Forms.Button btn8;
    private System.Windows.Forms.Button btn9;
    private System.Windows.Forms.Button btnMultiply;
    private System.Windows.Forms.Button btn4;
    private System.Windows.Forms.Button btn5;
    private System.Windows.Forms.Button btn6;
    private System.Windows.Forms.Button btnSubtract;
    private System.Windows.Forms.Button btn1;
    private System.Windows.Forms.Button btn2;
    private System.Windows.Forms.Button btn3;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Button btnNegate;
    private System.Windows.Forms.Button btn0;
    private System.Windows.Forms.Button btnDecimal;
    private System.Windows.Forms.Button btnEquals;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.txtDisplay = new System.Windows.Forms.TextBox();
        this.lblExpression = new System.Windows.Forms.Label();
        this.btnPercent = new System.Windows.Forms.Button();
        this.btnCE = new System.Windows.Forms.Button();
        this.btnC = new System.Windows.Forms.Button();
        this.btnBack = new System.Windows.Forms.Button();
        this.btnReciprocal = new System.Windows.Forms.Button();
        this.btnSquare = new System.Windows.Forms.Button();
        this.btnSqrt = new System.Windows.Forms.Button();
        this.btnDivide = new System.Windows.Forms.Button();
        this.btn7 = new System.Windows.Forms.Button();
        this.btn8 = new System.Windows.Forms.Button();
        this.btn9 = new System.Windows.Forms.Button();
        this.btnMultiply = new System.Windows.Forms.Button();
        this.btn4 = new System.Windows.Forms.Button();
        this.btn5 = new System.Windows.Forms.Button();
        this.btn6 = new System.Windows.Forms.Button();
        this.btnSubtract = new System.Windows.Forms.Button();
        this.btn1 = new System.Windows.Forms.Button();
        this.btn2 = new System.Windows.Forms.Button();
        this.btn3 = new System.Windows.Forms.Button();
        this.btnAdd = new System.Windows.Forms.Button();
        this.btnNegate = new System.Windows.Forms.Button();
        this.btn0 = new System.Windows.Forms.Button();
        this.btnDecimal = new System.Windows.Forms.Button();
        this.btnEquals = new System.Windows.Forms.Button();
        this.SuspendLayout();

        // txtDisplay
        this.txtDisplay.BackColor = System.Drawing.Color.White;
        this.txtDisplay.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtDisplay.Location = new System.Drawing.Point(12, 12);
        this.txtDisplay.Name = "txtDisplay";
        this.txtDisplay.ReadOnly = true;
        this.txtDisplay.Size = new System.Drawing.Size(336, 50);
        this.txtDisplay.TabIndex = 0;
        this.txtDisplay.Text = "0";
        this.txtDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

        // lblExpression
        this.lblExpression.AutoSize = false;
        this.lblExpression.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblExpression.ForeColor = System.Drawing.Color.Gray;
        this.lblExpression.Location = new System.Drawing.Point(12, 65);
        this.lblExpression.Name = "lblExpression";
        this.lblExpression.Size = new System.Drawing.Size(336, 20);
        this.lblExpression.TabIndex = 1;
        this.lblExpression.Text = "";
        this.lblExpression.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

        // Row 1: %, CE, C, Back
        this.btnPercent = CreateButton("%", 12, 95, 78, 45, System.Drawing.Color.FromArgb(240, 240, 240));
        this.btnCE = CreateButton("CE", 96, 95, 78, 45, System.Drawing.Color.FromArgb(240, 240, 240));
        this.btnC = CreateButton("C", 180, 95, 78, 45, System.Drawing.Color.FromArgb(240, 240, 240));
        this.btnBack = CreateButton("⌫", 264, 95, 84, 45, System.Drawing.Color.FromArgb(240, 240, 240));

        // Row 2: 1/x, x², √x, ÷
        this.btnReciprocal = CreateButton("1/x", 12, 146, 78, 45, System.Drawing.Color.FromArgb(240, 240, 240));
        this.btnSquare = CreateButton("x²", 96, 146, 78, 45, System.Drawing.Color.FromArgb(240, 240, 240));
        this.btnSqrt = CreateButton("√x", 180, 146, 78, 45, System.Drawing.Color.FromArgb(240, 240, 240));
        this.btnDivide = CreateButton("÷", 264, 146, 84, 45, System.Drawing.Color.FromArgb(255, 152, 0));

        // Row 3: 7, 8, 9, ×
        this.btn7 = CreateButton("7", 12, 197, 78, 45, System.Drawing.Color.White);
        this.btn8 = CreateButton("8", 96, 197, 78, 45, System.Drawing.Color.White);
        this.btn9 = CreateButton("9", 180, 197, 78, 45, System.Drawing.Color.White);
        this.btnMultiply = CreateButton("×", 264, 197, 84, 45, System.Drawing.Color.FromArgb(255, 152, 0));

        // Row 4: 4, 5, 6, −
        this.btn4 = CreateButton("4", 12, 248, 78, 45, System.Drawing.Color.White);
        this.btn5 = CreateButton("5", 96, 248, 78, 45, System.Drawing.Color.White);
        this.btn6 = CreateButton("6", 180, 248, 78, 45, System.Drawing.Color.White);
        this.btnSubtract = CreateButton("−", 264, 248, 84, 45, System.Drawing.Color.FromArgb(255, 152, 0));

        // Row 5: 1, 2, 3, +
        this.btn1 = CreateButton("1", 12, 299, 78, 45, System.Drawing.Color.White);
        this.btn2 = CreateButton("2", 96, 299, 78, 45, System.Drawing.Color.White);
        this.btn3 = CreateButton("3", 180, 299, 78, 45, System.Drawing.Color.White);
        this.btnAdd = CreateButton("+", 264, 299, 84, 45, System.Drawing.Color.FromArgb(255, 152, 0));

        // Row 6: ±, 0, ., =
        this.btnNegate = CreateButton("±", 12, 350, 78, 45, System.Drawing.Color.White);
        this.btn0 = CreateButton("0", 96, 350, 78, 45, System.Drawing.Color.White);
        this.btnDecimal = CreateButton(".", 180, 350, 78, 45, System.Drawing.Color.White);
        this.btnEquals = CreateButton("=", 264, 350, 84, 45, System.Drawing.Color.FromArgb(255, 152, 0));

        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        this.ClientSize = new System.Drawing.Size(360, 410);
        this.Controls.Add(this.lblExpression);
        this.Controls.Add(this.txtDisplay);
        this.Controls.Add(this.btnPercent);
        this.Controls.Add(this.btnCE);
        this.Controls.Add(this.btnC);
        this.Controls.Add(this.btnBack);
        this.Controls.Add(this.btnReciprocal);
        this.Controls.Add(this.btnSquare);
        this.Controls.Add(this.btnSqrt);
        this.Controls.Add(this.btnDivide);
        this.Controls.Add(this.btn7);
        this.Controls.Add(this.btn8);
        this.Controls.Add(this.btn9);
        this.Controls.Add(this.btnMultiply);
        this.Controls.Add(this.btn4);
        this.Controls.Add(this.btn5);
        this.Controls.Add(this.btn6);
        this.Controls.Add(this.btnSubtract);
        this.Controls.Add(this.btn1);
        this.Controls.Add(this.btn2);
        this.Controls.Add(this.btn3);
        this.Controls.Add(this.btnAdd);
        this.Controls.Add(this.btnNegate);
        this.Controls.Add(this.btn0);
        this.Controls.Add(this.btnDecimal);
        this.Controls.Add(this.btnEquals);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Calculator";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Button CreateButton(string text, int x, int y, int width, int height, System.Drawing.Color backColor)
    {
        var btn = new System.Windows.Forms.Button();
        btn.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        btn.Location = new System.Drawing.Point(x, y);
        btn.Name = "btn" + text.Replace("/", "").Replace("²", "2").Replace("√", "Sqrt").Replace("±", "Neg").Replace("⌫", "Back");
        btn.Size = new System.Drawing.Size(width, height);
        btn.TabIndex = 0;
        btn.Text = text;
        btn.UseVisualStyleBackColor = true;
        btn.BackColor = backColor;
        btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 220, 220);
        btn.Click += new System.EventHandler(this.Button_Click);
        return btn;
    }
}