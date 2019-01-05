using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.SampleMind
{

    public class RandomMind : AbstractPathMind
    {
        float[,] recompensas;
        float[,] calidad;
        public float alpha = 0.3f;
        public float gamma = 0.8f;
        bool auxiliarTemporal = true;
        void tablaRecompensas(BoardInfo boardInfo, CellInfo[] goals)
        {
            int x = 0;
            int y = 0;
            int _j = 0;
            //Variable auxiliar donde guardamos el valor
            float valor = 0;
            //info de la Cell a observar
            CellInfo cell_Obj = new CellInfo(-1, -1);
            //Recorremos las filas con i y las columnas con j segun la accion n
            for (int n = 0; n < 4; n++) 
                for (int i = 0; i < boardInfo.NumColumns; i++)
                    for (int j = 0; j < boardInfo.NumRows; j++)
                    {
                        valor = -1;
                        //evitamos calcular los puntos inaccesibles para no salir del array
                        if (n == 1 && j != 0)
                            if (n == 0 && j != boardInfo.NumRows - 1)
                                if (n == 2 && i != 0)
                                    if (n == 3 && i != boardInfo.NumColumns - 1)
                                    {
                                        x = i;
                                        y = j;
                                        switch (n)
                                        {
                                            case 0:
                                                y++;
                                                break;
                                            case 1:
                                                y--;
                                                break;
                                            case 2:
                                                x--;
                                                break;
                                            case 3:
                                                x++;
                                                break;
                                        }

                                        cell_Obj = boardInfo.CellInfos[x, y];
                                        //comprobamos primero si es caminable la cell

                                        if (cell_Obj.Walkable)
                                            valor = 0;
                                        //Si la cell es caminable comprobamos si es o no goal

                                        if (valor != -1)
                                            for (int l = 0; l < goals.Length; l++)
                                                if (cell_Obj.CellId == goals[l].CellId)
                                                    valor = 100;

                                        //Introducimos el valor en la tabla de recompensas
                                      
                                    }
                        recompensas[n, i + j * boardInfo.NumColumns] = valor;
                        
                    }
        }
        //este método inicia la tabla de calidad a 0
        void tablaCalidad(BoardInfo boardInfo)
        {
            for (int n = 0; n < 4; n++)
                for (int i = 0; i < boardInfo.NumColumns; i++)
                         for (int j = 0; j < boardInfo.NumRows; j++)
                    calidad[n,i + j*boardInfo.NumColumns] = 0;
                           
        }

        bool actualizar(BoardInfo bInfo,CellInfo currentPos, int accion, float alpha, float gamma)
        {
            //a=0 N a=1 S a=2 W a=3 E
            int x = currentPos.ColumnId;
            int y = currentPos.RowId;
            int j = x + y * bInfo.NumColumns;
            switch (accion)
            {
                case 0:
                    y++;
                    break;
                case 1:
                    y--;
                    break;
                case 2:
                    x--;
                    break;
                case 3:
                    x++;
                    break;
            }
            int _j = x + y * bInfo.NumColumns;
            //j = posicion en la que estamos. _j = posicion en la que estariamos de realizar la accion prevista.
            //Si la accion nos saca del mapa evitamos realizarla y la guardamos como -1
            if (recompensas[accion, j] == -1)
            {
                calidad[accion, j] = recompensas[accion, j];
                return false;
            }
            //calculamos la accion si no es inaccesible
            calidad[accion, j] = (1 - alpha) * calidad[accion, j] + alpha * (recompensas[accion, j] + gamma * Mathf.Max(calidad[0, _j], calidad[1, _j], calidad[2, _j], calidad[3, _j]));
            return true;
        }
        
        public override void Repath()
        {

        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            //Guarreo para inicializar las tablas
            if (auxiliarTemporal)
            {
                Iniciar(boardInfo, currentPos, goals);
                auxiliarTemporal = false;
            }
            int accion = Random.Range(0, 3);
            //Actualizar devuelve true si caminamos sobre una superficie caminable, sino no 
            while (actualizar(boardInfo,currentPos, accion, alpha, gamma))
                accion = Random.Range(0, 3);

            if (accion == 0) return Locomotion.MoveDirection.Up;
            if (accion == 1) return Locomotion.MoveDirection.Down;
            if (accion == 2) return Locomotion.MoveDirection.Left;
            return Locomotion.MoveDirection.Right;
        }


        void Iniciar(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            recompensas = new float[4, boardInfo.NumColumns * boardInfo.NumRows];
            calidad = new float[4, boardInfo.NumColumns * boardInfo.NumRows];
            tablaRecompensas(boardInfo, goals);
            tablaCalidad(boardInfo);
        }
    }
}