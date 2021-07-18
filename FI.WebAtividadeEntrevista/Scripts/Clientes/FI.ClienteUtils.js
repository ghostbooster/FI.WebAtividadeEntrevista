$("#CPF").inputmask("999.999.999-99");
$("#CPFBeneficiario").inputmask("999.999.999-99");


if (!indiceBeneficiarioSelecionado) {
    indiceBeneficiarioSelecionado = -1;
}
if (!beneficiarios) {
    beneficiarios = [];
}


$(document).on("click", ".adicionarBeneficiario", function () {
    var nomeBeneficiario = document.getElementById("NomeBeneficiario").value;
    var cpfBeneficiario = document.getElementById("CPFBeneficiario").value;

    cpfBeneficiario = cpfBeneficiario.replace(".", "");
    cpfBeneficiario = cpfBeneficiario.replace(".", "");
    cpfBeneficiario = cpfBeneficiario.replace("-", "");

    if (!ValidarBeneficiario(cpfBeneficiario, nomeBeneficiario)) {
        return;
    } 

    if (indiceBeneficiarioSelecionado < 0) {
       
        var beneficiario = {};
        beneficiario['Id'] = 0;
        beneficiario['CPF'] = cpfBeneficiario;
        beneficiario['Nome'] = nomeBeneficiario;

        beneficiarios.push(beneficiario);        

        adicionarBeneficiarioNoGrid(0, cpfBeneficiario, nomeBeneficiario);
    }
    else {
        beneficiarios[indiceBeneficiarioSelecionado].Nome = nomeBeneficiario;
        beneficiarios[indiceBeneficiarioSelecionado].CPF = cpfBeneficiario;

        indiceBeneficiarioSelecionado = -1;

        $('#gridBeneficiarios').find('tbody').empty();
        $('#gridBeneficiarios').append($('<tbody>'));

        document.getElementById("adicionarBeneficiario").innerHTML = 'Adicionar';
        document.getElementById("CPFBeneficiario").removeAttribute("disabled");

        PreencherBeneficiariosDoModelNoGrid();
    }

    document.getElementById("NomeBeneficiario").value = "";
    document.getElementById("CPFBeneficiario").value = "";
});

function adicionarBeneficiarioNoGrid(idBeneficiario, cpfBeneficiario, nomeBeneficiario) {
    var corpoDoGrid = document.getElementById('gridBeneficiarios').getElementsByTagName('tbody')[0];

    var novaLinha = corpoDoGrid.insertRow();
    novaLinha.innerHTML =
        '   <td style="display:none">' +
        '       <span>' + idBeneficiario + '</span>' +
        '   </td>' +
        '   <td>' +
        '       <p>'+ cpfBeneficiario + '</p>' +
        '   </td>' +
        '   <td>' +
        '       <p>' + nomeBeneficiario + '</p>' +
        '   </td>' +
        '   <td>' +
        '       <button type="button" class="btn btn-primary btn-sm m-0 alterarBeneficiario">Alterar</button>' +
        '       <button type="button" class="btn btn-primary btn-sm m-0 excluirBeneficiario">Excluir</button>' +
        '   </td>';
}

function ValidarBeneficiario(cpfBeneficiario, nomeBeneficiario) {

    if (cpfBeneficiario === "") {
        alert('O CPF do beneficiário deve ser informado');

        document.getElementById("CPFBeneficiario").focus();

        return false;
    }

    if (!ValidarCPF(cpfBeneficiario)) {
        alert('O CPF informado não é válido');

        document.getElementById("CPFBeneficiario").focus();

        return false;
    }   

    if (CPFBeneficiarioJaCadastrado(cpfBeneficiario)) {
        alert('O CPF do beneficiário já foi cadastrado');

        document.getElementById("CPFBeneficiario").focus();

        return false;
    }

    if (nomeBeneficiario === "") {
        alert('O nome do beneficiário deve ser informado');

        document.getElementById("NomeBeneficiario").focus();

        return false;
    }      

    return true;
}

function ValidarCPF(cpf) {
    var numeros, digitos, soma, i, resultado, digitos_iguais;
    digitos_iguais = 1;
    if (cpf.length < 11)
        return false;
    for (i = 0; i < cpf.length - 1; i++)
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (!digitos_iguais) {
        numeros = cpf.substring(0, 9);
        digitos = cpf.substring(9);
        soma = 0;
        for (i = 10; i > 1; i--)
            soma += numeros.charAt(10 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;
        numeros = cpf.substring(0, 10);
        soma = 0;
        for (i = 11; i > 1; i--)
            soma += numeros.charAt(11 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;

        if ((cpf === "12345678909") ||
            (cpf === "11111111111") ||
            (cpf === "22222222222") ||
            (cpf === "33333333333") ||
            (cpf === "44444444444") ||
            (cpf === "55555555555") ||
            (cpf === "66666666666") ||
            (cpf === "77777777777") ||
            (cpf === "88888888888") ||
            (cpf === "99999999999")) {
            return false;
        }
        return true;
    }
    else
        return false;
}

function CPFBeneficiarioJaCadastrado(cpfBeneficiario) {
    var corpoDoGrid = document.getElementById('gridBeneficiarios').getElementsByTagName('tbody')[0];

    for (var row of corpoDoGrid.rows) {
        var cpfAtual = row.cells[1].innerText;

        if ((cpfAtual === cpfBeneficiario) && ((indiceBeneficiarioSelecionado < 0) || (indiceBeneficiarioSelecionado !== row.rowIndex -1))) {
            return true;
        }
    }

    return false;
}

function PreencherBeneficiarioComInformacoesGrid() {
    var corpoDoGrid = document.getElementById('gridBeneficiarios').getElementsByTagName('tbody')[0];

    beneficiarios = [];

    for (var row of corpoDoGrid.rows) {
        var idAtual = parseInt(row.cells[0].innerText);
        var cpfAtual = row.cells[1].innerText;
        var nomeAtual = row.cells[2].innerText;

        cpfAtual = cpfAtual.replace(".", "");
        cpfAtual = cpfAtual.replace(".", "");
        cpfAtual = cpfAtual.replace("-", "");
        
        var beneficiario = {};
        beneficiario['Id'] = idAtual;
        beneficiario['CPF'] = cpfAtual.trim();
        beneficiario['Nome'] = nomeAtual.trim();

        beneficiarios.push(beneficiario);
    }
}

function PreencherBeneficiariosDoModelNoGrid() {
    for (var i = 0; i < beneficiarios.length; i++) {
        var beneficiario = beneficiarios[i];      

        adicionarBeneficiarioNoGrid(beneficiario.Id, beneficiario.CPF, beneficiario.Nome);
    }
}

$(document).on("click", ".alterarBeneficiario", function () {
    var linhaSelecionada = $(this).closest('tr');       

    var gridBeneficiarios = document.getElementById('gridBeneficiarios');

    var cpfAtual = gridBeneficiarios.rows[linhaSelecionada[0].rowIndex].cells[1].innerText;
    var nomeAtual = gridBeneficiarios.rows[linhaSelecionada[0].rowIndex].cells[2].innerText;

    cpfAtual = cpfAtual.replace(".", "");
    cpfAtual = cpfAtual.replace(".", "");
    cpfAtual = cpfAtual.replace("-", "");

    document.getElementById("NomeBeneficiario").value = nomeAtual;
    document.getElementById("CPFBeneficiario").value = cpfAtual;

    indiceBeneficiarioSelecionado = linhaSelecionada[0].rowIndex - 1;

    document.getElementById("adicionarBeneficiario").innerHTML = 'Alterar';
    document.getElementById("CPFBeneficiario").setAttribute("disabled", "disabled");
});

$('#popupBeneficiarios').on('hidden.bs.modal', function () {
    indiceBeneficiarioSelecionado = -1;

    document.getElementById("adicionarBeneficiario").innerHTML = 'Adicionar';

    document.getElementById("NomeBeneficiario").value = "";
    document.getElementById("CPFBeneficiario").value = "";

    document.getElementById("CPFBeneficiario").removeAttribute("disabled");
})


$(document).on("click", ".excluirBeneficiario", function () {    
    var linhaSelecionada = $(this).closest('tr'); 

    var indiceBeneficiarioRemovido = linhaSelecionada[0].rowIndex - 1; 

    var beneficiarioRemovido = beneficiarios[indiceBeneficiarioRemovido];

    beneficiarios.splice(indiceBeneficiarioRemovido, 1)

    beneficiariosRemovidos.push(beneficiarioRemovido);

    $('#gridBeneficiarios').find('tbody').empty();
    $('#gridBeneficiarios').append($('<tbody>')); 

    PreencherBeneficiariosDoModelNoGrid();
})