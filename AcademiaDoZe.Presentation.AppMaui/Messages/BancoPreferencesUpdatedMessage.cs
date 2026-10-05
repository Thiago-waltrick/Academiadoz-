using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Messages
{
    // Mensagem enviada quando a preferência de banco for alterada.
    public class BancoPreferencesUpdatedMessage : ValueChangedMessage<string>
    {
        public BancoPreferencesUpdatedMessage(string value) : base(value) { }
    }
}
