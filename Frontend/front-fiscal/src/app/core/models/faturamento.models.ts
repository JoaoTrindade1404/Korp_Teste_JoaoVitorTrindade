export interface NotaFiscalCreateDTO {
    itens: ItemNotaFiscalCreateDTO[];
}

export interface ItemNotaFiscalCreateDTO {
    produtoId: string;
    quantidade: number;
    nomeProduto: string;
}

export interface NotaFiscalResponseDTO {
    id: string;
    numeroSequencial: number;
    status: string;
    itens: ItemNotaFiscalResponseDTO[];
}

export interface ItemNotaFiscalResponseDTO {
    produtoId: string;
    nomeProduto: string;
    quantidade: number;
}

export interface RespostaIA {
  sucesso: boolean;
  mensagem: string;
  itens: { produtoId: string; quantidade: number }[];
}