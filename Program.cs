using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
internal class PadariaPaoPrecioso
{

    struct Produto
    {
        public string Descricao;
        public int Quantidade;
        public float Preco;
        public DateTime Validade;
    }
    struct Venda
    {
        public string Produto;
        public int Quantidade;
        public float ValorTotal;
        public string FormaPagamento;
    }

    static Produto[] estoque = new Produto[100];
    static Venda[] vendas = new Venda[100];
    static int contadorProdutos = 0;
    static int contadorVendas = 0;
    private static void Main(string[] args)
    {
        int opcao;

        do
        {
            ExibirMenu();
            opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    CadastrarProduto();
                    break;
                case 2:
                    RealizarVenda();
                    break;
                case 3:
                    ListarEstoque();
                    break;
                case 4:
                    FecharCaixa();
                    break;
                case 5:
                    SalvarDadosEmArquivo();
                    break;
                case 0:
                    Console.WriteLine("Saindo do sistema...");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente");
                    break;
            }
        } while (opcao != 0);
    }

    static void ExibirMenu()
    {
        Console.WriteLine("\nMENU - Padaria Pão Precioso");
        Console.WriteLine("1 - Cadastrar Produto");
        Console.WriteLine("2 - Realizar Venda");
        Console.WriteLine("3 - Listar Estoque");
        Console.WriteLine("4 - Fechar Caixa");
        Console.WriteLine("5 - Salvar Estoque em Arquivo");
        Console.WriteLine("6 - Gerar Relatórios");
        Console.WriteLine("0 - Sair");
        Console.Write("Escolha uma opção: ");
    }

    static void CadastrarProduto()
    {
        if (contadorProdutos >= estoque.Length)
        {
            Console.WriteLine("Capacidade máxima de estoque atingida");
            return;
        }
        Produto p;
        Console.Write("Descrição do produto: ");
        p.Descricao = Console.ReadLine();
        Console.Write("Quantidade em estoque: ");
        p.Quantidade = int.Parse(Console.ReadLine());
        Console.Write("Preço do produto: ");
        p.Preco = float.Parse(Console.ReadLine());
        Console.Write("Validade (dd/mm/yyyy): ");
        p.Validade = DateTime.Parse(Console.ReadLine());

        estoque[contadorProdutos++] = p;

        Console.WriteLine("Produto cadastrado com sucesso");
    }

    static void RealizarVenda()
    {
        Console.Write("Digite o nome do produto: ");
        string produto = Console.ReadLine();

        Array.Sort(estoque, 0, contadorProdutos, Comparer<Produto>.Create((x, y) => x.Descricao.CompareTo(y.Descricao)));
        int indice = BuscaBinaria(produto, 0, contadorProdutos - 1);

        if (indice == -1)
        {
            Console.WriteLine("Produto não encontrado");
            return;
        }

        Console.Write("Quantidade vendida: ");
        int quantidade = int.Parse(Console.ReadLine());

        if (estoque[indice].Quantidade < quantidade)
        {
            Console.WriteLine("Estoque insuficiente");
            return;
        }

        Console.Write("Forma de pagamento (Dinheiro/Cartão): ");
        string formaPagamento = Console.ReadLine();

        estoque[indice].Quantidade -= quantidade;

        vendas[contadorVendas++] = new Venda
        {
            Produto = produto,
            Quantidade = quantidade,
            ValorTotal = quantidade * estoque[indice].Preco,
            FormaPagamento = formaPagamento
        };

        Console.WriteLine("Venda realizada com sucesso");
    }

    static int BuscaBinaria(string produto, int inicio, int fim)
    {
        if (inicio > fim)
            return -1;

        int meio = (inicio + fim) / 2;

        if (estoque[meio].Descricao == produto)
            return meio;

        if (string.Compare(produto, estoque[meio].Descricao) < 0)
            return BuscaBinaria(produto, inicio, meio - 1);

        return BuscaBinaria(produto, meio + 1, fim);
    }

    static void ListarEstoque()
    {
        Console.WriteLine("\nEstoque: ");
        for (int i = 0; i < contadorProdutos; i++)
        {
            Console.WriteLine($"Produto: {estoque[i].Descricao}, Quantidade: {estoque[i].Quantidade}, Preço: R$ {estoque[i].Preco:0.00}, Validade: {estoque[i].Validade:dd/MM/yyyy}");
        }
    }

    static void FecharCaixa()
    {
        float total = vendas.Take(contadorVendas).Sum(vendas => v.ValorTotal);
        Console.WriteLine($"\nTotal do dia: R$ {total:0.00}");
    }

    static void SalvarDadosEmArquivo()
    {
        using (FileStream fs = new FileStream("estoque.bin", FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            for (int i = 0; i < contadorProdutos; i++)
            {
                writer.Write(estoque[i].Descricao);
                writer.Write(estoque[i].Quantidade);
                writer.Write(estoque[i].Preco);
                writer.Write(estoque[i].Validade.ToString("o"));
            }
        }
        Console.WriteLine("Estoque salvo em estoque.bin");
    }

    static void GerarRelatorios()
    {
        using (StreamWriter estoqueRelatorio = new StreamWriter("estoque.txt"))
        using (StreamWriter vendasRelatorio = new StreamWriter("vendas.txt"))
        {
            estoqueRelatorio.WriteLine("Relatório de estoque");
            for (int i = 0; i < contadorProdutos; i++)
            {
                estoqueRelatorio.WriteLine($"Produto: {estoque[i].Descricao}, Quantidade: {estoque[i].Quantidade}, Validade: {estoque[i].Validade:dd/MM/yyyy}");
            }

            vendasRelatorio.WriteLine("Relatório de vendas");
            for (int i = 0; i < contadorVendas; i++)
            {
                vendasRelatorio.WriteLine($"Produto: {vendas[i].Produto}, Quantidade: {vendas[i].Quantidade}, Valor: R$ {vendas[i].ValorTotal:0.00}, Pagamento: {vendas[i].FormaPagamento}");
            }
        }

        Console.WriteLine("Relatórios gerados em estoque.txt e vendas.txt");
    }
}