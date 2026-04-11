export interface ProdutoResponseDTO {
    id: string; 
    codigo: string;
    descricao: string;
    saldo: number; 
}

export interface ProdutoCreateDTO {
    codigo: string;
    descricao: string;
    saldo: number;
}