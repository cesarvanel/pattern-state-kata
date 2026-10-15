using LoanCase.Domain.Enum;
using LoanCase.Domain.Validator;

namespace LoanCase.Domain.Vo;

public class AttachedDocument
{
    public string Name { get; private set; } = string.Empty;
    public float Size { get; private set; } = 0;
    public string Extension { get; private set; } = string.Empty;
    public AttachedDocumentType Type { get; private set; }
    
    private AttachedDocument(string name, float size, string extension, AttachedDocumentType type)
    {
        Name = name;
        Size = size;
        Extension = extension;
        Type = type;
    }

    public static AttachedDocument Create(string name, float size, string extension, AttachedDocumentType type)
    {
        var validator = new DomainValidator<AttachedDocument>();
        validator
            .Ensure(!string.IsNullOrWhiteSpace(name), "Le nom du document ne peut pas être vide.")
            .Ensure(size > 0, "La taille du document doit être supérieure à zéro.")
            .Ensure(!string.IsNullOrWhiteSpace(extension), "L'extension du document est obligatoire.")
            .Ensure(extension.StartsWith('.'), "L'extension doit commencer par un point.");

        validator.ThrowIfInvalid();
        return new AttachedDocument(string.Empty, size, extension, type);
    }
    
}