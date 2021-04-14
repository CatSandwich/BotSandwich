using System;

namespace BotSandwich.Data
{
    class ArgumentBuilder
    {
        private readonly Argument _argument;
        private bool _name;
        private bool _callback;

        public ArgumentBuilder()
        {
            _argument = new Argument();
        }

        public Argument Build()
        {
            if (!_name) throw new InvalidOperationException("Cannot build an argument with no name.");
            if (!_callback) throw new InvalidOperationException("Cannot build an argument with no callback.");
            return _argument;
        }

        public ArgumentBuilder Required()
        {
            _argument.Required = true;
            return this;
        }

        public ArgumentBuilder WithNames(params string[] s)
        {
            _argument.Name = s;
            _name = true;
            return this;
        }

        public ArgumentBuilder WithDescription(string desc)
        {
            _argument.Description = desc;
            return this;
        }

        public ArgumentBuilder WithCallback(Argument.InvokeSig callback)
        {
            _argument.Invoke = callback;
            _callback = true;
            return this;
        }
    }
}
