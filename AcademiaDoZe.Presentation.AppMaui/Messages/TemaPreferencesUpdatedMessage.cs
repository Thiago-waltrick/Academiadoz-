using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Messages
{
    // Mensagem enviada quando a preferência de tema for alterada.
    public class TemaPreferencesUpdatedMessage : ValueChangedMessage<string>
    {
        public TemaPreferencesUpdatedMessage(string value) : base(value) { }
    }
}
