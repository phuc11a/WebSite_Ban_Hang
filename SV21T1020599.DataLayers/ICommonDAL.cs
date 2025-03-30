namespace SV21T1020599.DataLayers
{
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang(bằng 0 tức là không phân trang)</param>
        /// <param name="searchValue">Giá trị cần tìm (chuỗi rỗng tức là lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        List<T> List(int page = 1, int pageSize = 0, string searchValue = "");


        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm được 
        /// </summary>
        /// <param name="seachValue">Giá trị cần tìm (chuỗi rỗng nếu đếm toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");

        /// <summary>
        /// Lấy một dòng dữ liệu dựa vào id (trả về null nếu dữ liệu không tồn tại)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary> 
        /// Bổ sung một bản ghi vào CSDL. Hàm trả về ID của một dữ liệu bổ sung 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);


        /// <summary>
        /// Xóa dòng dữ liệu dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// Kiểm trang một dòng dữ liệu có khóa id hiện có dữ liệu tham chiếu hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);



    }
}