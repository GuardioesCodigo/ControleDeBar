$repo = "GuardioesCodigo/ControleDeBar"

$issues = @(
    "01|Padronizar estrutura e documentação do projeto",
    "02|Configurar GitHub Projects",
    "03|Criar CONTRIBUTING.md",
    "04|Criar CODE_OF_CONDUCT.md",
    "05|Configurar Branch Protection",
    "06|Configurar Templates de Issues",
    "07|Configurar Template de Pull Request",
    "08|Padronizar nomenclatura das branches",
    "09|Revisar arquitetura da solução",
    "10|Mapear dependências do projeto",
    "11|Atualizar AutoMapper para versão compatível",
    "12|Remover configurações inseguras",
    "13|Externalizar secrets do projeto",
    "14|Configurar User Secrets",
    "15|Configurar variáveis de ambiente",
    "16|Corrigir configuração New Relic",
    "17|Configurar Health Checks",
    "18|Implementar tratamento global de exceções",
    "19|Implementar logging estruturado",
    "20|Cobertura de testes unitários",
    "21|Cobertura de testes de integração",
    "22|Cobertura de testes E2E",
    "23|Configurar relatório de cobertura",
    "24|Padronizar FluentValidation",
    "25|Revisar entidades de domínio",
    "26|Revisar mapeamentos EF Core",
    "27|Otimizar migrations",
    "28|Validar LocalDB",
    "29|Configurar SQL Server produção",
    "30|Pipeline Build GitHub Actions",
    "31|Pipeline Test GitHub Actions",
    "32|Pipeline Quality Gate",
    "33|Pipeline Security Scan",
    "34|Pipeline Deploy Homologação",
    "35|Pipeline Deploy Produção",
    "36|Documentar CI/CD",
    "37|Criar backlog técnico V1",
    "38|Criar backlog técnico V2",
    "39|Preparar Release Candidate",
    "40|Preparar versão 1.0"
)

foreach ($issue in $issues) {
    $dados = $issue.Split("|")
    $numero = $dados[0]
    $titulo = $dados[1]

    Write-Host "Criando Issue $numero..."

    gh issue create `
        --repo $repo `
        --title "$numero - $titulo" `
        --body @"
## Objetivo
Implementar: $titulo

## Tarefas
- [ ] Analisar cenário atual
- [ ] Implementar solução
- [ ] Validar funcionamento
- [ ] Atualizar documentação

## Critérios de aceite
- [ ] Funcionalidade concluída
- [ ] Testes executados
- [ ] Documentação atualizada
"@
}