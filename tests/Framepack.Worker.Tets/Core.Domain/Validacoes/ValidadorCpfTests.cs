using Core.Domain.Validacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framepack.Worker.Tets.Core.Domain.Validacoes;

public class ValidadorCpfTests
{
    [Theory]
    [InlineData("111.111.111-11", false)] // CPF com dígitos repetidos
    [InlineData("529.982.247-25", true)]  // CPF válido
    [InlineData("52998224725", true)]     // CPF válido sem caracteres especiais
    [InlineData("529-982-247.25", true)]  // CPF válido com caracteres especiais misturados
    [InlineData("000.000.000-00", false)] // CPF com dígitos repetidos
    [InlineData("529.982.247-2", false)]  // CPF com tamanho inválido
    public void Validar_Should_ReturnExpectedResult(string cpf, bool expectedResult)
    {
        // Act
        var result = ValidadorCpf.Validar(cpf);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Validar_Should_ReturnFalse_When_CpfIsEmpty()
    {
        // Arrange
        var cpf = string.Empty;

        // Act
        var result = ValidadorCpf.Validar(cpf);

        // Assert
        Assert.False(result);
    }
}