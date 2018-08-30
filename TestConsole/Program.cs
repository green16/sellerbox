using System.ComponentModel;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            VkConnector.Models.Common.Keyboard keyboard = new VkConnector.Models.Common.Keyboard()
            {
                OneTime = true,
                Buttons = new VkConnector.Models.Common.Keyboard.Button[][]
                {
                    new VkConnector.Models.Common.Keyboard.Button[]
                    {
                        new VkConnector.Models.Common.Keyboard.Button()
                        {
                            Action = new VkConnector.Models.Common.Keyboard.ButtonAction()
                            {
                                Text = "Red",
                                Payload = "{\"RedButton\":1}"
                            },
                            Color = "negative"
                        },
                        new VkConnector.Models.Common.Keyboard.Button()
                        {
                            Action = new VkConnector.Models.Common.Keyboard.ButtonAction()
                            {
                                Text = "Green",
                                Payload = "{\"GreenButton\":1}"
                            },
                            Color = "positive"
                        }
                    },
                    new VkConnector.Models.Common.Keyboard.Button[]
                    {
                        new VkConnector.Models.Common.Keyboard.Button()
                        {
                            Action = new VkConnector.Models.Common.Keyboard.ButtonAction()
                            {
                                Text = "White",
                                Payload = "{\"WhiteButton\":1}"
                            },
                            Color = "default"
                        },
                        new VkConnector.Models.Common.Keyboard.Button()
                        {
                            Action = new VkConnector.Models.Common.Keyboard.ButtonAction()
                            {
                                Text = "Blue",
                                Payload = "{\"BlueButton\":1}"
                            },
                            Color = "primary"
                        }
                    }
                }
            };

            var task = VkConnector.Methods.Messages.Send("4d232e201bb5572df2651d89f5cc2f6ef84aa7c836c78697c6d17afdf1e73bd7a97e887ca33f32317c4fc", "Клавиатура", null, keyboard, new int[] { 3100102 });
            task.Wait();
        }
    }
}
