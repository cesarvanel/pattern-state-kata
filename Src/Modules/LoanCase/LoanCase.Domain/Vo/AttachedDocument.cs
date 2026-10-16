using LoanCase.Domain.Enum;

using Shared.Domain.Validator;

namespace LoanCase.Domain.Vo;

public record AttachedDocument
{
    private AttachedDocument(string name, float size, string extension, AttachedDocumentType type)
    {
        Name = name;
        Size = size;
        Extension = extension;
        Type = type;
    }

    public string Name { get; }
    public float Size { get; }
    public string Extension { get; }
    public AttachedDocumentType Type { get; }

    public static AttachedDocument Create(string name,
        float size,
        string extension,
        AttachedDocumentType type)
    {
        string[] allowedExtensions = [".pdf", ".png", ".jpeg", ".jpg"];
        DomainValidator<AttachedDocument> validator = new();
        validator
            .Ensure(!string.IsNullOrWhiteSpace(name), "Le nom du document ne peut pas être vide.")
            .Ensure(size > 0, "La taille du document doit être supérieure à zéro.")
            .Ensure(!string.IsNullOrWhiteSpace(extension),
                "L'extension du document est obligatoire.")
            .Ensure(extension.StartsWith('.'), "L'extension doit commencer par un point.").Ensure(
                allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase),
                $"Format de document non autorisé {extension} . Seuls les fichiers ${string.Join(" ,", allowedExtensions).ToUpper()} sont acceptés."
            );

        validator.ThrowIfInvalid();
        return new AttachedDocument(string.Empty, size, extension, type);
    }
}