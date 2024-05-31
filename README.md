<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>APC Injection Tool</title>
</head>
<body>

<h1>APC Injection Tool</h1>

<p>Este projeto em C# demonstra várias técnicas avançadas de injeção de código em processos do Windows usando chamadas de API nativas. É destinado para fins educacionais e de teste em ambientes controlados.</p>

<h2>Descrição</h2>

<p>Este programa é uma ferramenta de injeção de código que implementa diferentes métodos de injeção em processos do Windows. Abaixo estão as principais funções e seus propósitos:</p>

<h3>Funções Principais</h3>

<h4>SelfTestAlert</h4>
<p>Esta função aloca memória no processo atual, escreve um payload nessa memória e utiliza uma chamada de APC (Asynchronous Procedure Call) para tentar executar o payload no contexto do próprio thread do programa. Esta função é usada para testar a lógica de injeção de código localmente, sem envolver outros processos.</p>

<h4>EarlyBirdAPC</h4>
<p>Esta função cria um novo processo (<code>notepad.exe</code>) em estado suspenso, aloca memória nesse processo, escreve o payload na memória alocada e então injeta uma chamada APC no thread principal do novo processo. Finalmente, retoma a execução do thread para ativar o payload. Permite a injeção de código em um novo processo de maneira controlada.</p>

<h4>QueueUserAPC</h4>
<p>Esta função destina-se a injetar um payload em um thread específico de um processo existente. Aloca memória no processo alvo, escreve o payload nessa memória e injeta uma chamada APC no thread especificado. Pode ser usada para inserir código em processos já existentes no sistema.</p>

<h2>Fluxo do Programa</h2>

<ol>
    <li>Definição de um payload em formato de array de bytes, que contém código de shell.</li>
    <li>Dependendo da função chamada (<code>SelfTestAlert</code>, <code>EarlyBirdAPC</code> ou <code>QueueUserAPC</code>), o payload é injetado e executado de diferentes maneiras:
        <ul>
            <li><strong>SelfTestAlert</strong>: Testa a lógica de injeção dentro do próprio processo.</li>
            <li><strong>EarlyBirdAPC</strong>: Cria um novo processo e injeta o payload nele.</li>
            <li><strong>QueueUserAPC</strong>: Injeta o payload em um thread específico de um processo existente.</li>
        </ul>
    </li>
</ol>

<h2>Notas Importantes</h2>

<ul>
    <li><strong>Segurança</strong>: Este programa demonstra técnicas avançadas de injeção de código que podem ser usadas de maneira maliciosa. Deve ser usado apenas em ambientes de teste controlados e com propósitos educativos.</li>
    <li><strong>Dependências</strong>: O programa utiliza chamadas de API do Windows através de métodos P/Invoke para realizar operações de alocação de memória, escrita de memória e manipulação de threads.</li>
    <li><strong>Ambiente de Execução</strong>: Foi originalmente escrito para o .NET Framework, mas foi atualizado para usar .NET Core, permitindo maior portabilidade e compatibilidade com diferentes versões do Windows.</li>
</ul>

<h2>Exemplo de Uso</h2>

<pre><code>public static void Main(string[] args)
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
</code></pre>

<h2>Contribuição</h2>

<p>Sinta-se à vontade para contribuir com melhorias ou correções. Por favor, abra um issue ou envie um pull request.</p>

<h2>Licença</h2>

<p>Este projeto está licenciado sob a Licença MIT. Veja o arquivo <code>LICENSE</code> para mais detalhes.</p>

</body>
</html>
