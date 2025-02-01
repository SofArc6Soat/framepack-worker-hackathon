using Core.Domain.Base;
using Core.Domain.Entities;
using Core.Domain.Notificacoes;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Framepack.Worker.Tets.Core.Domain.Base;

public class BaseUseCaseTests
{
    private readonly Mock<INotificador> _notificadorMock;
    private readonly BaseUseCaseTestClass _baseUseCase;

    public BaseUseCaseTests()
    {
        _notificadorMock = new Mock<INotificador>();
        _baseUseCase = new BaseUseCaseTestClass(_notificadorMock.Object);
    }

    [Fact]
    public void Notificar_DeveChamarHandleComMensagem()
    {
        // Arrange
        var mensagem = "Erro de validação";

        // Act
        _baseUseCase.ChamarNotificar(mensagem);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(n => n.Mensagem == mensagem)), Times.Once);
    }

    [Fact]
    public void Notificar_DeveChamarHandleComErrosDeValidacao()
    {
        // Arrange
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Propriedade", "Erro de validação 1"),
            new ValidationFailure("Propriedade", "Erro de validação 2")
        });

        // Act
        _baseUseCase.ChamarNotificar(validationResult);

        // Assert
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(n => n.Mensagem == "Erro de validação 1")), Times.Once);
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(n => n.Mensagem == "Erro de validação 2")), Times.Once);
    }

    [Fact]
    public void ExecutarValidacao_DeveRetornarTrueQuandoValidacaoForValida()
    {
        // Arrange
        var entidade = new TestEntity { Nome = "Nome Válido" }; // Nome é obrigatório e válido
        var validator = new TestEntityValidator();

        // Act
        var resultado = _baseUseCase.ChamarExecutarValidacao(validator, entidade);

        // Assert
        Assert.True(resultado);
        _notificadorMock.Verify(n => n.Handle(It.IsAny<Notificacao>()), Times.Never);
    }

    [Fact]
    public void ExecutarValidacao_DeveRetornarFalseQuandoValidacaoForInvalida()
    {
        // Arrange
        var entidade = new TestEntity { Nome = null }; // Nome é obrigatório
        var validator = new TestEntityValidator();

        // Act
        var resultado = _baseUseCase.ChamarExecutarValidacao(validator, entidade);

        // Assert
        Assert.False(resultado);
        _notificadorMock.Verify(n => n.Handle(It.Is<Notificacao>(n => n.Mensagem == "Nome é obrigatório")), Times.Once);
    }

    private class BaseUseCaseTestClass : BaseUseCase
    {
        public BaseUseCaseTestClass(INotificador notificador) : base(notificador) { }

        public void ChamarNotificar(string mensagem) => Notificar(mensagem);

        public void ChamarNotificar(ValidationResult validationResult) => Notificar(validationResult);

        public bool ChamarExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            return ExecutarValidacao(validacao, entidade);
        }
    }

    private class TestEntity : Entity
    {
        public string Nome { get; set; }
    }

    private class TestEntityValidator : AbstractValidator<TestEntity>
    {
        public TestEntityValidator()
        {
            RuleFor(e => e.Nome).NotEmpty().WithMessage("Nome é obrigatório");
        }
    }
}
