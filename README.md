APC Injection Tool

Este projeto em C# demonstra várias técnicas avançadas de injeção de código em processos do Windows usando chamadas de API nativas. É destinado para fins educacionais e de teste em ambientes controlados.
Descrição

Este programa é uma ferramenta de injeção de código que implementa diferentes métodos de injeção em processos do Windows. Abaixo estão as principais funções e seus propósitos:
Funções Principais
SelfTestAlert

Esta função aloca memória no processo atual, escreve um payload nessa memória e utiliza uma chamada de APC (Asynchronous Procedure Call) para tentar executar o payload no contexto do próprio thread do programa. Esta função é usada para testar a lógica de injeção de código localmente, sem envolver outros processos.
EarlyBirdAPC

Esta função cria um novo processo (notepad.exe) em estado suspenso, aloca memória nesse processo, escreve o payload na memória alocada e então injeta uma chamada APC no thread principal do novo processo. Finalmente, retoma a execução do thread para ativar o payload. Permite a injeção de código em um novo processo de maneira controlada.
QueueUserAPC

Esta função destina-se a injetar um payload em um thread específico de um processo existente. Aloca memória no processo alvo, escreve o payload nessa memória e injeta uma chamada APC no thread especificado. Pode ser usada para inserir código em processos já existentes no sistema.
Fluxo do Programa

    Definição de um payload em formato de array de bytes, que contém código de shell.
    Dependendo da função chamada (SelfTestAlert, EarlyBirdAPC ou QueueUserAPC), o payload é injetado e executado de diferentes maneiras:
        SelfTestAlert: Testa a lógica de injeção dentro do próprio processo.
        EarlyBirdAPC: Cria um novo processo e injeta o payload nele.
        QueueUserAPC: Injeta o payload em um thread específico de um processo existente.

Notas Importantes

    Segurança: Este programa demonstra técnicas avançadas de injeção de código que podem ser usadas de maneira maliciosa. Deve ser usado apenas em ambientes de teste controlados e com propósitos educativos.
    Dependências: O programa utiliza chamadas de API do Windows através de métodos P/Invoke para realizar operações de alocação de memória, escrita de memória e manipulação de threads.
    Ambiente de Execução: Foi originalmente escrito para o .NET Framework, mas foi atualizado para usar .NET Core, permitindo maior portabilidade e compatibilidade com diferentes versões do Windows.

Exemplo de Uso

csharp

public static void Main(string[] args)
{
    byte[] payload = new byte[] {
        0xfc,0x48,0x83,0xe4,0xf0,0xe8,0xc0,0x00,0x00,0x00,0x41,0x51,0x41,0x50,0x52,
        // ... restante do payload
    };

    // Teste local da lógica de injeção
    SelfTestAlert(payload);

    // Injeção de código em um novo processo (notepad.exe)
    EarlyBirdAPC(payload);

    // Injeção de código em um thread específico de um processo existente
    QueueUserAPC(processId, threadId, payload);
}
