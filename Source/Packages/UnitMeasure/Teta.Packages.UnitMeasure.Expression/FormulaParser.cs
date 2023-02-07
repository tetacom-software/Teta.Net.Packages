using Teta.Packages.UnitMeasure.Expression.Models;
using Teta.Packages.UnitMeasure.Expression.Tokens;

namespace Teta.Packages.UnitMeasure.Expression
{
	/// <summary>
	/// A one-pass parser of simple mathematical expressions (+,-,*,/) with nested brackets without using functions
	/// </summary>
	public class FormulaParser
	{
		private string _source;
		private Stack<Token> _stack;

		private bool _inOperand;
		private int _startOperandIdx = -1;

		private ExpressionToken _currExpr;
		private ExpressionToken _nextExpr;

		private Token[] _buffer;
		private int _tokenCnt;
		private List<ExpressionToken> _order;
		private Formula _formula;

		public Formula Parse(string source)
		{
			_tokenCnt = 0;
			_formula = new Formula { Source = source };
			_buffer = new Token[4];
			_source = source;
			_stack = new Stack<Token>();
			_order = new List<ExpressionToken>();

			for (var i = 0; i < _source.Length; i++)
			{
				var sym = _source[i];
				switch (sym)
				{
					case (char)9: // horizontal tab
					case (char)32: // space
					case (char)10: // NL line feed, new line (LF)
					case (char)13: // carriage return (CR)
						EndOperand(i);
						continue;

					case '(':
						// put the current expression on the stack
						if (!_inOperand)
						{
							if (_tokenCnt > 0)
							{
								((ExpressionToken)_buffer[1]).Left = (ValueToken)_buffer[0];
								_stack.Push(_buffer[1]);
							}

							_tokenCnt = 0;
							_stack.Push(new OpenBracketToken());
						}
						else
						{
							throw new NotSupportedException();

							// function call
							EndOperand(i);

							_buffer[_tokenCnt - 1] = new FunctionToken(_buffer[_tokenCnt - 1].ToString());
							if (_tokenCnt == 3)
							{
								((ExpressionToken)_buffer[1]).Left = (ValueToken)_buffer[0];
								_stack.Push(_buffer[1]);
							}

							_stack.Push(_buffer[_tokenCnt - 1]);
							_tokenCnt = 0;
							_stack.Push(new OpenBracketToken());
						}

						continue;

					case ')':
						EndOperand(i);
						_nextExpr = (ExpressionToken)_buffer[1];
						_nextExpr.Left = (ValueToken)_buffer[0];
						_nextExpr.Right = (ValueToken)_buffer[2];
						Token token;
						while ((token = _stack.Pop()).TokenType != TokenType.OpenBracket)
						{
							_order.Add(_nextExpr);
							_currExpr = (ExpressionToken)token;
							_currExpr.Right = _nextExpr;
							_nextExpr = _currExpr;
						}

						_nextExpr.Priority = OperationOrder.Bracket;
						if (_stack.Count > 0 && _stack.Peek().TokenType != TokenType.OpenBracket)
						{
							_order.Add(_nextExpr);
							_currExpr = (ExpressionToken)_stack.Pop();
							_currExpr.Right = _nextExpr;
							_buffer[1] = _currExpr;
							_buffer[0] = _currExpr.Left;
							_buffer[2] = _currExpr.Right;
							_tokenCnt = 3;
						}
						else
						{
							_order.Add(_nextExpr);
							_buffer[0] = _nextExpr;
							_tokenCnt = 1;
						}

						continue;

					case '+':
					case '-':
					case '/':
					case '*':
						EndOperand(i);
						_nextExpr = new ExpressionToken(sym);
						_buffer[_tokenCnt] = _nextExpr;
						if (_tokenCnt == 3)
						{
							ProcessBuffer();
							continue;
						}

						_tokenCnt++;
						break;

					default:
						StartOperand(i);
						continue;
				}
			}

			if (_inOperand)
			{
				EndOperand(_source.Length);
			}

			switch (_tokenCnt)
			{
				case 1:
					_nextExpr = (ExpressionToken)_buffer[0];
					_order.Add(_nextExpr);
					break;
				case 3:
					_nextExpr = (ExpressionToken)_buffer[1];
					_nextExpr.Left = (ValueToken)_buffer[0];
					_nextExpr.Right = (ValueToken)_buffer[2];
					_order.Add(_nextExpr);
					break;
			}

			while (_stack.Count > 0)
			{
				_currExpr = (ExpressionToken)_stack.Pop();
				_currExpr.Right = _nextExpr;
				_order.Add(_currExpr);
				_nextExpr = _currExpr;
			}

			_formula.Order = _order;
			return _formula;
		}

		private void StartOperand(int index)
		{
			if (_inOperand)
			{
				return;
			}

			_inOperand = true;
			_startOperandIdx = index;
		}

		private void EndOperand(int index)
		{
			if (!_inOperand)
			{
				return;
			}

			var currentOperand = _source.Substring(_startOperandIdx, index - _startOperandIdx);
			ValueToken operand;

			var sep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			var tmp = currentOperand.Replace(sep == "." ? "," : ".", sep);

			if (double.TryParse(tmp, out var numeric))
			{
				operand = new NumberToken { Value = numeric };
			}
			else
			{
				if (!_formula.Operands.TryGetValue(currentOperand, out operand))
				{
					operand = new VariableToken(currentOperand);
					_formula.Operands.Add(currentOperand, operand);
				}
			}

			_buffer[_tokenCnt++] = operand;
			_inOperand = false;
		}

		private void ProcessBuffer()
		{
			_currExpr = (ExpressionToken)_buffer[1];
			_nextExpr = (ExpressionToken)_buffer[3];

			if (_currExpr.Priority >= _nextExpr.Priority)
			{
				_currExpr.Right = (ValueToken)_buffer[2];
				_currExpr.Left = (ValueToken)_buffer[0];

				while (_stack.Count > 0 && (_stack.Peek().TokenType != TokenType.OpenBracket) &&
				       _stack.Peek().Priority >= _nextExpr.Priority)
				{
					_order.Add(_currExpr);
					_currExpr = (ExpressionToken)_stack.Pop();
					_buffer[1] = _currExpr;
				}

				// if priority the current operation over than the next, push it into the queue
				_order.Add(_currExpr);

				// move buffer
				_buffer[0] = _buffer[1]; // _currExpr;
				_buffer[1] = _buffer[3]; // _nextExpr;
				_tokenCnt = 2;
			}
			else
			{
				// add to stack
				_stack.Push(_currExpr);
				_currExpr.Left = (ValueToken)_buffer[0];
				_currExpr.Right = (ValueToken)_buffer[3];

				// move buffer
				_buffer[0] = _buffer[2]; // operand
				_buffer[1] = _buffer[3]; // _nextExpr;
				_tokenCnt = 2;
			}
		}
	}
}
