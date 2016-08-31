using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Calculator : KeyboardListener {
    public Text display;

    string input = "";
    char operation = '\0';
    double op1 = 0.0;
    double op2 = 0.0;
    double result = 0.0;
    bool showResult = false;

    private void ProcessKey(string key)
    {
        this.showResult = false;
        switch(key)
        {
            case "del":
                this.Backspace();
                break;
            case "clr":
                this.Clear();
                break;
            case "times":
                this.SetOperand('*');
                break;
            case "div":
                this.SetOperand('/');
                break;
            case "plus":
                this.SetOperand('+');
                break;
            case "minus":
                this.SetOperand('-');
                break;
            case "=":
                this.Execute();
                break;
            case "ans":
                // Execute with the last result as operand 2
                if (this.operation != '\0')
                {
                    this.DoExecute(this.result);
                }
                break;
            default:
                this.input = this.input.Insert(input.Length, key);
                break;
        }

        this.UpdateDisplay();
    }

    private void Backspace()
    {
        if (this.input.Length > 0)
        {
            this.input = this.input.Remove(this.input.Length - 1);
        }
        else
        {
            this.showResult = true;
        }
    }

    private void Clear()
    {
        this.input = "";
        this.op1 = 0.0;
        this.op2 = 0.0;
        this.result = 0.0;
        this.operation = '\0';
    }

    private void SetOperand(char op)
    {
        if (double.TryParse(this.input, out this.op1))
        {
            this.operation = op;
            this.input = "";
        }
        else
        {
            // empty or invalid input, use existing result
            this.operation = op;
            this.op1 = this.result;
        }
    }

    private void Execute()
    {
        if (double.TryParse(this.input, out this.op2))
        {
            this.DoExecute(this.op2);
        }
        else
        {
            // empty or invalid input, show existing result
            this.input = "";
            this.operation = '\0';
            this.showResult = true;
        }
    }

    private void DoExecute(double op2)
    {
        this.op2 = op2;
        switch (this.operation)
        {
            case '+':
                this.result = this.op1 + this.op2;
                break;
            case '-':
                this.result = this.op1 - this.op2;
                break;
            case '/':
                if (this.op2 != 0.0f)
                {
                    this.result = this.op1 / this.op2;
                }
                else
                {
                    this.result = float.NaN;
                }
                break;
            case '*':
                this.result = this.op1 * this.op2;
                break;
            case '\0':
                this.result = this.op2;
                break;
        }

        this.input = "";
        this.operation = '\0';
        this.showResult = true;
    }

    // Updates the visible display of the VR calculator
    private void UpdateDisplay()
    {
        if (this.operation == '\0')
        {
            if (this.showResult)
            {
                this.display.text = string.Format("={0:G15}", this.result);
            }
            else
            {
                this.display.text = this.input;
            }
        }
        else
        {
            if (this.input == "")
            {
                this.display.text = string.Format("{0:G15}{1}", this.op1, this.operation);
            }
            else
            {
                this.display.text = string.Format("{0:G15}{1}\n{2}\n", this.op1, this.operation, this.input);
            }
        }
    }

    public override void OnKeyUp(PhysicsKeyboard.PhysicsKey key)
    {
        base.OnKeyUp(key);
        this.ProcessKey(key.normal);
    }
}
