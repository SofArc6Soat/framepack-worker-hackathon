namespace Core.Domain.Validacoes
{
    public static class ValidadorCpf
    {
        public const int TamanhoCpf = 11;

        public static bool Validar(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return false;
            }

            var cpfNumeros = RemoverCaracteresEspeciais(cpf);

            return TamanhoValido(cpfNumeros) &&
                   !TemDigitosRepetidos(cpfNumeros) &&
                   TemDigitosValidos(cpfNumeros);
        }

        private static string RemoverCaracteresEspeciais(string cpf) =>
            new(cpf.Where(char.IsDigit).ToArray());

        private static bool TamanhoValido(string cpf) =>
            cpf.Length == TamanhoCpf;

        private static bool TemDigitosRepetidos(string cpf)
        {
            var invalidNumbers = new[]
            {
                    "00000000000", "11111111111", "22222222222", "33333333333",
                    "44444444444", "55555555555", "66666666666", "77777777777",
                    "88888888888", "99999999999"
                };

            return invalidNumbers.Contains(cpf);
        }

        private static bool TemDigitosValidos(string cpf)
        {
            var number = cpf[..(TamanhoCpf - 2)];
            var digitoVerificador = new DigitoVerificador(number)
                .ComMultiplicadoresDeAte(2, 11)
                .Substituindo("0", 10, 11);
            var firstDigit = digitoVerificador.CalculaDigito();
            digitoVerificador.AddDigito(firstDigit);
            var secondDigit = digitoVerificador.CalculaDigito();

            return string.Concat(firstDigit, secondDigit) == cpf.Substring(TamanhoCpf - 2, 2);
        }
    }

    internal class DigitoVerificador
    {
        private const int Modulo = 11;
        private readonly List<int> _multiplicadores = [2, 3, 4, 5, 6, 7, 8, 9];
        private readonly Dictionary<int, string> _substituicoes = [];
        private readonly bool _complementarDoModulo = true;
        private string _numero;

        public DigitoVerificador(string numero) => _numero = numero;

        public DigitoVerificador ComMultiplicadoresDeAte(int primeiroMultiplicador, int ultimoMultiplicador)
        {
            _multiplicadores.Clear();

            for (var i = primeiroMultiplicador; i <= ultimoMultiplicador; i++)
            {
                _multiplicadores.Add(i);
            }

            return this;
        }

        public DigitoVerificador Substituindo(string substituto, params int[] digitos)
        {
            foreach (var i in digitos)
            {
                _substituicoes[i] = substituto;
            }

            return this;
        }

        public void AddDigito(string digito) =>
            _numero = string.Concat(_numero, digito);

        public string CalculaDigito() => (_numero.Length <= 0) ? "" : GetDigitSum();

        private string GetDigitSum()
        {
            var soma = 0;
            for (int i = _numero.Length - 1, m = 0; i >= 0; i--)
            {
                var produto = (int)char.GetNumericValue(_numero[i]) * _multiplicadores[m];
                soma += produto;

                if (++m >= _multiplicadores.Count)
                {
                    m = 0;
                }
            }

            var mod = soma % Modulo;
            var resultado = _complementarDoModulo ? Modulo - mod : mod;

            return _substituicoes.TryGetValue(resultado, out var value) ? value : resultado.ToString();
        }
    }
}
