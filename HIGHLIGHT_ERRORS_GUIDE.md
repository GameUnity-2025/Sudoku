# Hướng Dẫn Cài Đặt Tính Năng "Highlight Errors"

## Tính năng đã thêm:
- **Tự động đánh dấu lỗi**: Các ô nhập sai sẽ được highlight màu đỏ nhạt
- **Hiển thị lỗi cùng hàng/cột**: Các ô xung đột cùng hàng, cùng cột, hoặc cùng ô 3x3 sẽ được highlight
- **Nút Show/Hide Mistakes**: Cho phép người chơi bật/tắt tính năng hiển thị lỗi

## Các thay đổi code:

### 1. SudokuCell.cs
- Thêm biến `cellImage`, `originalColor`, `errorColor` để quản lý highlight
- Thêm phương thức `HighlightError()` - Đánh dấu ô lỗi màu đỏ
- Thêm phương thức `ClearError()` - Xóa đánh dấu lỗi
- Thêm getter `GetRow()` và `GetCol()` để lấy vị trí ô

### 2. Board.cs
- Thêm mảng `allCells[9,9]` để lưu tham chiếu đến tất cả các ô
- Thêm biến `autoCheckErrors` để bật/tắt tự động kiểm tra lỗi
- Cập nhật `CreateButtons()` để lưu tham chiếu ô vào mảng
- Thêm `CheckAndHighlightErrors()` - Kiểm tra và highlight tất cả lỗi
- Thêm `HighlightConflictingCells()` - Highlight các ô xung đột
- Thêm `ClearAllErrors()` - Xóa tất cả highlight lỗi
- Thêm `SetAutoCheckErrors()` - Bật/tắt tự động kiểm tra lỗi
- Cập nhật `UpdatePuzzle()` để tự động kiểm tra lỗi sau mỗi lần nhập

### 3. ShowMistakesButton.cs (Mới)
- Script để gắn vào nút UI "Show Mistakes"
- `ToggleMistakes()` - Chuyển đổi chế độ hiển thị lỗi
- Tự động cập nhật text của nút thành "Show Mistakes" hoặc "Hide Mistakes"

## Cách sử dụng trong Unity:

### Bước 1: Cập nhật SudokuCell Prefab
1. Mở Prefab `SudokuCell_Prefab` trong folder `Assets/Prefabs/`
2. Đảm bảo GameObject có component **Image** (để có thể đổi màu)
3. Nếu chưa có, thêm component Image vào GameObject chính của prefab

### Bước 2: Thêm nút "Show Mistakes" vào UI
1. Mở scene `SudokuPlay` trong `Assets/Scenes/`
2. Tạo Button mới trong Canvas:
   - Right-click Canvas → UI → Button
   - Đặt tên: "ShowMistakesButton"
3. Đặt vị trí phù hợp (ví dụ: góc trên bên phải)
4. Thêm component `ShowMistakesButton` script vào Button
5. Trong Inspector của ShowMistakesButton:
   - Kéo thả Text component của button vào field `Button Text`
6. Trong Button component:
   - Thêm OnClick event
   - Kéo GameObject ShowMistakesButton vào ô Object
   - Chọn function: `ShowMistakesButton.ToggleMistakes()`

### Bước 3: Test
1. Chạy game
2. Nhập một số sai vào ô bất kỳ
3. Ô đó và các ô xung đột sẽ được highlight màu đỏ nhạt
4. Click nút "Hide Mistakes" để ẩn highlight
5. Click nút "Show Mistakes" để hiện lại highlight

## Tùy chỉnh màu lỗi:
Trong `SudokuCell.cs`, dòng 18:
```csharp
private Color errorColor = new Color(1f, 0.7f, 0.7f); // Light red
```
Bạn có thể thay đổi giá trị RGB để có màu khác:
- `new Color(1f, 0.5f, 0.5f)` - Đỏ đậm hơn
- `new Color(1f, 0.9f, 0.5f)` - Màu cam/vàng
- `new Color(1f, 0.8f, 0.8f)` - Hồng nhạt

## Lưu ý:
- Tính năng mặc định BẬT (autoCheckErrors = true)
- Chỉ highlight các ô SAI so với đáp án chính xác
- Highlight cả ô sai và các ô xung đột cùng hàng/cột/ô 3x3
